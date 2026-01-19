using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Infrastructure.Caching.Services;

namespace Unjai.Platform.Infrastructure.Caching.Extensions;

public static class CachingExtension
{
    public static void AddCachingExtension(this IServiceCollection services)
    {
        services.AddHybridCache();

        services.AddHostedService<CacheInvalidationService>();
    }
}
