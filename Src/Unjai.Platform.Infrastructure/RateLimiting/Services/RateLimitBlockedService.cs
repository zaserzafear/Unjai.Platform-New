using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Unjai.Platform.Infrastructure.Messaging.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting.Services;

internal sealed class RateLimitBlockedService(
    ILogger<RateLimitBlockedService> logger,
    IConnectionMultiplexer multiplexer,
    IMemoryCache cache)
    : BackgroundService
{
    private readonly RedisChannel Channel = RedisChannel.Literal(RateLimitBlockedConfig.Channel);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = multiplexer.GetSubscriber();

        await subscriber.SubscribeAsync(Channel, (channel, value) =>
        {
            string message = value.ToString();

            var payload = JsonSerializer.Deserialize<RateLimitBlockedMessage>(message)!;
            string key = payload.Key;
            TimeSpan retryAfter = TimeSpan.FromSeconds(payload.TtlSeconds);

            logger.LogInformation("Received rate-limit block for key: {Key}, ttl: {Ttl}s",
                key,
                payload.TtlSeconds);

            cache.Set(key, true, retryAfter);
        });
    }
}

public static class RateLimitBlockedConfig
{
    public const string Channel = "rate-limit-blocked";
}
