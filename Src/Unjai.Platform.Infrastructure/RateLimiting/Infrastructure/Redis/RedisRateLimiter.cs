using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using Unjai.Platform.Infrastructure.Messaging.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Redis;

internal sealed class RedisRateLimiter(
    IConnectionMultiplexer multiplexer,
    IMemoryCache cache,
    IDistributedNotificationPublisher publisher)
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();

    // Lua script:
    // - Atomically increments the counter
    // - Sets expiration on first hit
    // - Returns both the current count and remaining TTL
    private const string Script = """
    local current = redis.call("INCR", KEYS[1])

    if current == 1 then
        redis.call("EXPIRE", KEYS[1], ARGV[1])
    end

    local ttl = redis.call("TTL", KEYS[1])

    return { current, ttl }
    """;

    public async Task<bool> IsAllowedAsync(
        string key,
        int limit,
        TimeSpan window)
    {
        // Fast path:
        // If the key is already blocked in-memory, short-circuit to avoid hitting Redis.
        if (cache.TryGetValue(key, out _))
        {
            return false;
        }

        // Slow path:
        // Execute the Redis Lua script to perform the authoritative rate-limit check.
        var evalResult = await _redis.ScriptEvaluateAsync(
            Script,
            keys: [key],
            values: [(int)window.TotalSeconds]);

        // Defensive fail-open:
        // If Redis returns null (unexpected), allow the request to avoid hard outages.
        if (evalResult.IsNull)
        {
            return true;
        }

        var result = (RedisResult[])evalResult!;

        var count = (long)result[0];
        var ttlSeconds = (long)result[1];

        // If the request count is within the limit, allow it.
        if (count <= limit)
        {
            return true;
        }

        // Denied path:
        // Publish a distributed rate-limit block notification with the remaining TTL.
        // Downstream subscribers (e.g. in-memory guards, gateways) can use this signal
        // to short-circuit repeated requests without hitting Redis.
        var retryAfter = ttlSeconds >= 0
            ? TimeSpan.FromSeconds(ttlSeconds)
            : window; // Fallback in case TTL is missing

        await publisher.NotifyRateLimitBlockedAsync(key, retryAfter);

        return false;
    }
}
