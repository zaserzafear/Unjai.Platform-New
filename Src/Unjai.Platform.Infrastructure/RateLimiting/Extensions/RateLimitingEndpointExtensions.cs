using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
                    factoryContext.ApplicationServices.GetRequiredService<RedisRateLimiter>(),
                    factoryContext.ApplicationServices.GetRequiredService<IRateLimitPolicyResolver>(),
                    policyName);

                return await filter.InvokeAsync(invocationContext, next);
            };
        });

        return builder;
    }
}

