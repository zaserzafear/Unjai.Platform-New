using Microsoft.AspNetCore.Http;
using Unjai.Platform.Infrastructure.RateLimiting.Core;

namespace Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Filters;

internal sealed class RedisRateLimitFilter(
        RateLimitEnforcer enforcer,
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
            return Results.StatusCode(StatusCodes.Status429TooManyRequests);
        }

        return await next(context);
    }
}
