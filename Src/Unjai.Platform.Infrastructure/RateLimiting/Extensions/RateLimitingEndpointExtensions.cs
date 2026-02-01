using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Filters;
using Unjai.Platform.Infrastructure.RateLimiting.Core;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

public static class RateLimitingEndpointExtensions
{
    public static RouteHandlerBuilder EnforceRateLimit(
        this RouteHandlerBuilder builder,
        string policyName)
    {
        builder.AddEndpointFilterFactory((factoryContext, next) =>
        {
            return async invocationContext =>
            {
                var requestServices =
                    invocationContext.HttpContext.RequestServices;

                var filter = new RedisRateLimitFilter(
                    requestServices.GetRequiredService<RateLimitEnforcer>(),
                    requestServices.GetRequiredService<IMinimalRateLimitResultFactory>(),
                    policyName);

                return await filter.InvokeAsync(invocationContext, next);
            };
        });

        return builder;
    }
}
