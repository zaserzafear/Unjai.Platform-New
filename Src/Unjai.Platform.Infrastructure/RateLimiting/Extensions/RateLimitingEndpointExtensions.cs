using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Infrastructure.RateLimiting.Filters;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitingEndpointExtensions
{
    public static RouteHandlerBuilder RequireRateLimiting(
        this RouteHandlerBuilder builder,
        string policyName)
    {
        builder.AddEndpointFilterFactory((factoryContext, next) =>
        {
            return async invocationContext =>
            {
                var filter = new RedisRateLimitFilter(
                    factoryContext.ApplicationServices
                        .GetRequiredService<RateLimitEnforcer>(),
                    policyName);

                return await filter.InvokeAsync(invocationContext, next);
            };
        });

        return builder;
    }
}
