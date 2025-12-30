using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitingExtension
{
    public static void AddRateLimitingExtension(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddSingleton<RedisRateLimiter>();
        services.AddSingleton<IRateLimitPolicyResolver>(sp =>
        {
            var policies = new Dictionary<string, RateLimitPolicy>
            {
                ["get-user"] = new RateLimitPolicy(
                    Name: "get-user",
                    Limit: 5,
                    Window: TimeSpan.FromMinutes(1)),

                ["login"] = new RateLimitPolicy(
                    Name: "login",
                    Limit: 10,
                    Window: TimeSpan.FromSeconds(30))
            };

            return new FixedWindowPolicyResolver(policies);
        });

        services.AddSingleton<RateLimitEnforcer>();
    }
}
