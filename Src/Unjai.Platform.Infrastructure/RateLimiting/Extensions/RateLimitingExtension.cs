using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Delegates;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Redis;
using Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Security;
using Unjai.Platform.Infrastructure.RateLimiting.Policies;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitingExtension
{
    public static void AddRateLimitingExtension(
        this IServiceCollection services)
    {
        services.AddSingleton<RedisRateLimiter>();
        services.AddSingleton<RateLimitEnforcer>();
        services.AddTransient<RateLimitContextHandler>();

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

        services.AddSingleton<IRateLimitContextSigner>(sp =>
        {
            var options = sp
                .GetRequiredService<IOptions<RateLimitingOptions>>()
                .Value;

            var secret = Convert.FromBase64String(options.Secret);

            return new HmacRateLimitContextSigner(
                secret,
                options.ContextTtl);
        });
    }
}
