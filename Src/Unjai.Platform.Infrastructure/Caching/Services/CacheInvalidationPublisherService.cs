using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.Caching.Services;

internal sealed class CacheInvalidationPublisherService(
    ILogger<CacheInvalidationPublisherService> logger,
    IConnectionMultiplexer connectionMultiplexer
    ) : ICacheInvalidationPublisherService
{
    private static readonly RedisChannel CacheInvalidationChannel =
        RedisChannel.Literal(CacheInvalidationConfig.Channel);

    public async Task NotifyCacheInvalidationAsync(string key)
    {
        var subscriber = connectionMultiplexer.GetSubscriber();

        await subscriber.PublishAsync(CacheInvalidationChannel, new RedisValue(key));

        logger.LogInformation("Published invalidation for key: {Key}", key);
    }
}
