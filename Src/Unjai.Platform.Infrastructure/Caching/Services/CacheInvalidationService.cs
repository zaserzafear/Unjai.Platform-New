using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.Caching.Services;

internal sealed class CacheInvalidationService(
    ILogger<CacheInvalidationService> logger,
    IConnectionMultiplexer multiplexer,
    HybridCache hybridCache)
    : BackgroundService
{
    private readonly RedisChannel Channel = RedisChannel.Literal(CacheInvalidationConfig.Channel);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = multiplexer.GetSubscriber();

        await subscriber.SubscribeAsync(Channel, (channel, value) =>
        {
            string key = value.ToString();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Received invalidation for key: {Key}", key);
            }

            var task = hybridCache.RemoveAsync(key, stoppingToken);

            if (!task.IsCompleted)
            {
                task.GetAwaiter().GetResult();
            }
        });
    }
}

internal static class CacheInvalidationConfig
{
    public const string Channel = "cache-invalidation";
}
