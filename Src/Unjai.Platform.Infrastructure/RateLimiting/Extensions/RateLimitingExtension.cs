using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitingExtension
{
    public static void AddRateLimitingExtension(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddSingleton<RedisRateLimiter>();
        services.AddSingleton<IRateLimitPolicyResolver, FixedWindowPolicyResolver>();
    }
}
