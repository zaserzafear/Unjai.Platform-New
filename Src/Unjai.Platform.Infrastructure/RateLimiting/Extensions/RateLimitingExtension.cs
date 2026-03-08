using Microsoft.Extensions.DependencyInjection;
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
        this IServiceCollection services,
        Action<RateLimitingOptions> rateLimitingOptions)
    {
        var options = new RateLimitingOptions();
        rateLimitingOptions(options);

        ValidateRateLimitingOptions(options);

        services.Configure(rateLimitingOptions);

        services.AddSingleton<RedisRateLimiter>();
        services.AddSingleton<RateLimitEnforcer>();
        services.AddTransient<RateLimitContextHandler>();

        services.AddSingleton<IRateLimitBlockedPublisherService, RedisRateLimitBlockedPublisherService>();
        services.AddHostedService<RateLimitBlockedService>();

        services.AddSingleton<IRateLimitPolicyResolver>(sp =>
        {
            var policies = new Dictionary<string, RateLimitPolicy>();

            foreach (var (key, policy) in options.Policies)
            {
                policies[key] = new RateLimitPolicy(
                    Name: key,
                    Strategy: policy.Strategy,
                    Limit: policy.Limit,
                    Window: policy.Window,
                    TokensPerPeriod: policy.TokensPerPeriod,
                    ReplenishmentPeriod: policy.ReplenishmentPeriod);
            }

            return new PolicyResolver(policies);
        });

        services.AddSingleton<IRateLimitContextSigner>(sp =>
        {
            var secret = Convert.FromBase64String(options.Secret);

            return new HmacRateLimitContextSigner(
                secret,
                options.ContextTtl);
        });
    }

    private static void ValidateRateLimitingOptions(RateLimitingOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Secret))
            throw new InvalidOperationException("RateLimiting:Secret is required.");
    }
}
