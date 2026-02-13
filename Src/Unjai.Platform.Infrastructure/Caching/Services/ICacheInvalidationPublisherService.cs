namespace Unjai.Platform.Infrastructure.Caching.Services;

public interface ICacheInvalidationPublisherService
{
    Task NotifyCacheInvalidationAsync(string key);
}
