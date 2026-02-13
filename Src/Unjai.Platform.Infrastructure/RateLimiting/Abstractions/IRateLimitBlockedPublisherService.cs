namespace Unjai.Platform.Infrastructure.RateLimiting.Abstractions;

public interface IRateLimitBlockedPublisherService
{
    Task NotifyRateLimitBlockedAsync(
        string key,
        TimeSpan ttl);
}
