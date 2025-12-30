using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed class RedisRateLimiter(IConnectionMultiplexer multiplexer)
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();

    private const string Script = """
    local current = redis.call("INCR", KEYS[1])
    if current == 1 then
        redis.call("EXPIRE", KEYS[1], ARGV[1])
    end
    return current
    """;

    public async Task<bool> IsAllowedAsync(
        string key,
        int limit,
        TimeSpan window)
    {
        var count = (long)await _redis.ScriptEvaluateAsync(
            Script,
            keys: [key],
            values: [(int)window.TotalSeconds]);

        return count <= limit;
    }
}
