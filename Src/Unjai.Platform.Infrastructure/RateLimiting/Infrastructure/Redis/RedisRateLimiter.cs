using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using Unjai.Platform.Infrastructure.Messaging.Redis;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;

namespace Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Redis;

internal sealed class RedisRateLimiter(
    IConnectionMultiplexer multiplexer,
    IMemoryCache cache,
    IDistributedNotificationPublisher publisher)
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();

    #region Lua Scripts

    private const string FixedWindowScript = """
        local current = redis.call("INCR", KEYS[1])

        if current == 1 then
            redis.call("EXPIRE", KEYS[1], ARGV[1])
        end

        local ttl = redis.call("TTL", KEYS[1])

        return { current, ttl }
        """;

    private const string SlidingWindowScript = """
        local now = tonumber(ARGV[1])
        local window = tonumber(ARGV[2])
        local limit = tonumber(ARGV[3])

        redis.call(
            "ZREMRANGEBYSCORE",
            KEYS[1],
            0,
            now - window
        )

        local count = redis.call("ZCARD", KEYS[1])

        if count < limit then
            redis.call("ZADD", KEYS[1], now, now)
            redis.call("PEXPIRE", KEYS[1], window)
            return { 1, count + 1 }
        end

        return { 0, count }
        """;

    private const string TokenBucketScript = """
        local now = tonumber(ARGV[1])
        local capacity = tonumber(ARGV[2])
        local tokensPerPeriod = tonumber(ARGV[3])
        local period = tonumber(ARGV[4])

        local data = redis.call("HMGET", KEYS[1], "tokens", "timestamp")

        local tokens = tonumber(data[1])
        local lastRefill = tonumber(data[2])

        if tokens == nil then
            tokens = capacity
            lastRefill = now
        else
            local delta = math.max(0, now - lastRefill)
            local refill = math.floor(delta / period) * tokensPerPeriod
            tokens = math.min(capacity, tokens + refill)

            if refill > 0 then
                lastRefill = now
            end
        end

        if tokens <= 0 then
            return { 0, tokens }
        end

        tokens = tokens - 1

        redis.call("HMSET",
            KEYS[1],
            "tokens", tokens,
            "timestamp", lastRefill
        )

        redis.call("PEXPIRE", KEYS[1], period * 2)

        return { 1, tokens }
        """;

    #endregion

    public async Task<bool> IsAllowedAsync(
        string key,
        RateLimitStrategy strategy,
        int limit,
        TimeSpan window,
        int? tokensPerPeriod,
        TimeSpan? replenishmentPeriod)
    {
        // Fast-path: short-circuit from in-memory block cache
        if (cache.TryGetValue(key, out _))
        {
            return false;
        }

        return strategy switch
        {
            RateLimitStrategy.FixedWindow =>
                await ExecuteFixedWindowAsync(key, limit, window),

            RateLimitStrategy.SlidingWindow =>
                await ExecuteSlidingWindowAsync(key, limit, window),

            RateLimitStrategy.TokenBucket =>
                await ExecuteTokenBucketAsync(
                    key,
                    limit,
                    tokensPerPeriod,
                    replenishmentPeriod),

            _ => throw new NotSupportedException(
                $"Rate limit strategy '{strategy}' is not supported.")
        };
    }

    #region Strategy Implementations

    private async Task<bool> ExecuteFixedWindowAsync(
        string key,
        int limit,
        TimeSpan window)
    {
        var result = await _redis.ScriptEvaluateAsync(
            FixedWindowScript,
            keys: [key],
            values: [(int)window.TotalSeconds]);

        if (result.IsNull)
        {
            return true; // fail-open
        }

        var data = (RedisResult[])result!;
        var count = (long)data[0];
        var ttlSeconds = (long)data[1];

        if (count <= limit)
        {
            return true;
        }

        await NotifyBlockedAsync(key, ttlSeconds, window);
        return false;
    }

    private async Task<bool> ExecuteSlidingWindowAsync(
        string key,
        int limit,
        TimeSpan window)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var result = await _redis.ScriptEvaluateAsync(
            SlidingWindowScript,
            keys: [key],
            values:
            [
                now,
                (long)window.TotalMilliseconds,
                limit
            ]);

        if (result.IsNull)
        {
            return true;
        }

        var data = (RedisResult[])result!;
        var allowed = (long)data[0] == 1;

        if (allowed)
        {
            return true;
        }

        await NotifyBlockedAsync(key, null, window);
        return false;
    }

    private async Task<bool> ExecuteTokenBucketAsync(
        string key,
        int capacity,
        int? tokensPerPeriod,
        TimeSpan? replenishmentPeriod)
    {
        if (tokensPerPeriod is null || replenishmentPeriod is null)
        {
            throw new InvalidOperationException(
                "TokenBucket strategy requires TokensPerPeriod and ReplenishmentPeriod.");
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var result = await _redis.ScriptEvaluateAsync(
            TokenBucketScript,
            keys: [key],
            values:
            [
                now,
                capacity,
                tokensPerPeriod.Value,
                (long)replenishmentPeriod.Value.TotalMilliseconds
            ]);

        if (result.IsNull)
        {
            return true;
        }

        var data = (RedisResult[])result!;
        var allowed = (long)data[0] == 1;

        if (allowed)
        {
            return true;
        }

        await NotifyBlockedAsync(key, null, replenishmentPeriod.Value);
        return false;
    }

    #endregion

    #region Helpers

    private async Task NotifyBlockedAsync(
        string key,
        long? ttlSeconds,
        TimeSpan fallback)
    {
        var retryAfter = ttlSeconds.HasValue && ttlSeconds.Value >= 0
            ? TimeSpan.FromSeconds(ttlSeconds.Value)
            : fallback;

        await publisher.NotifyRateLimitBlockedAsync(key, retryAfter);
    }

    #endregion
}
