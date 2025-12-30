using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed class RedisRateLimiter(IConnectionMultiplexer multiplexer)
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();

    public async Task<bool> IsAllowedAsync(
        string key,
        int limit,
        TimeSpan window)
    {
        var count = await _redis.StringIncrementAsync(key);

        if (count == 1)
        {
            await _redis.KeyExpireAsync(key, window);
        }

        return count <= limit;
    }
}
