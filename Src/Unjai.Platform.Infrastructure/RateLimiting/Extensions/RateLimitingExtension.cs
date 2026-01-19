using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Services;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitingExtension
{
    public static void AddRateLimitingExtension(
        this IServiceCollection services)
    {
        services.AddSingleton<RedisRateLimiter>();
        services.AddSingleton<RateLimitEnforcer>();

        services.AddHostedService<RateLimitBlockedService>();

        services.AddSingleton<IRateLimitPolicyResolver>(sp =>
        {
            var options = sp
                .GetRequiredService<IOptions<RateLimitingOptions>>()
                .Value;

            var policies = new Dictionary<string, RateLimitPolicy>();

            foreach (var (key, policy) in options.Policies)
            {
                policies[key] = new RateLimitPolicy(
                    Name: key,
                    Limit: policy.Limit,
                    Window: policy.Window);
            }

            return new FixedWindowPolicyResolver(policies);
        });
    }
}
