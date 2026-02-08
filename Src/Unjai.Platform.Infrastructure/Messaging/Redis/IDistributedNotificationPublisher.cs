namespace Unjai.Platform.Infrastructure.Messaging.Redis;

public interface IDistributedNotificationPublisher
{
    Task NotifyCacheInvalidationAsync(string key);

    Task NotifyRateLimitBlockedAsync(
        string key,
        TimeSpan ttl);
}
