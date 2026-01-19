namespace Unjai.Platform.Infrastructure.Messaging.Redis;

internal interface IDistributedNotificationPublisher
{
    Task NotifyCacheInvalidationAsync(string key);

    Task NotifyRateLimitBlockedAsync(
        string key,
        TimeSpan ttl);
}
