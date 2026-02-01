using Microsoft.AspNetCore.Http;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Core;

namespace Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Filters;

internal sealed class RedisRateLimitFilter(
        RateLimitEnforcer enforcer,
        IMinimalRateLimitResultFactory resultFactory,
        string policyName
    ) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var result = await enforcer.EnforceAsync(
            context.HttpContext,
            policyName);

        if (!result.IsAllowed)
        {
            return resultFactory
                .CreateTooManyRequestsResult(context.HttpContext);
        }

        return await next(context);
    }
}
