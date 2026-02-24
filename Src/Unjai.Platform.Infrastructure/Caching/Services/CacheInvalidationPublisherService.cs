using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Unjai.Platform.Application.Abstractions.Caching;

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

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Published invalidation for key: {Key}", key);
        }
    }
}
