namespace Unjai.Platform.Application.Abstractions.Caching;

public interface ICacheInvalidationPublisherService
{
    Task NotifyCacheInvalidationAsync(string key);
}
