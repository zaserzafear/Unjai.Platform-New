using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitExtensions
{
    public static RouteHandlerBuilder RequireRateLimit(
        this RouteHandlerBuilder builder,
        string policyName)
    {
        builder.AddEndpointFilterFactory((ctx, next) =>
        {
            return invocationContext =>
            {
                var limiter = invocationContext.HttpContext
                    .RequestServices
                    .GetRequiredService<RedisRateLimiter>();

                var resolver = invocationContext.HttpContext
                    .RequestServices
                    .GetRequiredService<IRateLimitPolicyResolver>();

                var rejectionHandler = invocationContext.HttpContext
                    .RequestServices
                    .GetRequiredService<IRateLimitRejectionHandler>();

                var filter = new RedisRateLimitFilter(
                    limiter,
                    resolver,
                    rejectionHandler,
                    policyName);

                return filter.InvokeAsync(invocationContext, next);
            };
        });

        return builder;
    }
}
