using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Infrastructure.RateLimiting.Services;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitingExtension
{
    public static void AddRateLimitingExtension(this IServiceCollection services)
    {
        services.AddSingleton<RedisRateLimiter>();
        services.AddSingleton<RateLimitEnforcer>();

        services.AddHostedService<RateLimitBlockedService>();

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
    }
}
