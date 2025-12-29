using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting;

public sealed class RedisRateLimiter(IConnectionMultiplexer multiplexer)
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();

    public async Task<bool> IsAllowedAsync(
        string key,
        int limit,
        TimeSpan window)
    {
        const string script = """
        local current = redis.call("INCR", KEYS[1])
        if tonumber(current) == 1 then
            redis.call("PEXPIRE", KEYS[1], ARGV[1])
        end
        return current
        """;

        var result = (long)await _redis.ScriptEvaluateAsync(
            script,
            new RedisKey[] { key },
            new RedisValue[] { (long)window.TotalMilliseconds });

        return result <= limit;
    }
}
