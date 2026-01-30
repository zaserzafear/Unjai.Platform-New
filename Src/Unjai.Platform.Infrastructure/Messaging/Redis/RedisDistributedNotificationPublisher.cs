using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Unjai.Platform.Infrastructure.Caching.Services;
using Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Redis;

namespace Unjai.Platform.Infrastructure.Messaging.Redis;

internal sealed class RedisDistributedNotificationPublisher(
    IConnectionMultiplexer connectionMultiplexer,
    ILogger<RedisDistributedNotificationPublisher> logger)
    : IDistributedNotificationPublisher
{
    private static readonly RedisChannel CacheInvalidationChannel =
        RedisChannel.Literal(CacheInvalidationConfig.Channel);

    private static readonly RedisChannel RateLimitChannel =
        RedisChannel.Literal(RateLimitBlockedConfig.Channel);

    public async Task NotifyCacheInvalidationAsync(string key)
    {
        var subscriber = connectionMultiplexer.GetSubscriber();

        await subscriber.PublishAsync(CacheInvalidationChannel, new RedisValue(key));

        logger.LogInformation("Published invalidation for key: {Key}", key);
    }

    public async Task NotifyRateLimitBlockedAsync(
    string key,
    TimeSpan ttl)
    {
        var subscriber = connectionMultiplexer.GetSubscriber();

        var payload = new RateLimitBlockedMessage(
            key,
            (int)ttl.TotalSeconds);

        await subscriber.PublishAsync(
            RateLimitChannel,
            JsonSerializer.Serialize(payload));

        logger.LogInformation(
            "Published rate-limit block for key: {Key}, ttl: {Ttl}s",
            key,
            payload.TtlSeconds);
    }
}
