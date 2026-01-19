using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Unjai.Platform.Infrastructure.RateLimiting.Filters;

internal sealed class RedisRateLimitMvcFilter(
        RateLimitEnforcer enforcer,
        string policyName) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var result = await enforcer.EnforceAsync(
            context.HttpContext,
            policyName);

        if (!result.IsAllowed)
        {
            context.Result =
                new StatusCodeResult(StatusCodes.Status429TooManyRequests);
            return;
        }

        await next();
    }
}
