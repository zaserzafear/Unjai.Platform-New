using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Abstractions.Caching;
using Unjai.Platform.Infrastructure.Caching.Services;

namespace Unjai.Platform.Infrastructure.Caching.Extensions;

public static class CachingExtension
{
    public static void AddCachingExtension(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(1),
                Expiration = TimeSpan.FromMinutes(5),
            };
        });

        services.AddSingleton<ICacheInvalidationPublisherService, CacheInvalidationPublisherService>();
        services.AddHostedService<CacheInvalidationService>();
    }
}
