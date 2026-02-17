using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Context;

namespace Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Redis;

internal sealed class RedisRateLimitBlockedPublisherService(
    ILogger<RedisRateLimitBlockedPublisherService> logger,
    IConnectionMultiplexer connectionMultiplexer)
    : IRateLimitBlockedPublisherService
{
    private static readonly RedisChannel RateLimitChannel =
        RedisChannel.Literal(RateLimitBlockedConfig.Channel);

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

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Published rate-limit block for key: {Key}, ttl: {Ttl}s",
                key,
                payload.TtlSeconds);
        }
    }
}
