using Microsoft.AspNetCore.Mvc.Filters;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Core;

namespace Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Filters;

internal sealed class RedisRateLimitMvcFilter(
        RateLimitEnforcer enforcer,
        IMvcRateLimitResultFactory resultFactory,
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
                resultFactory.CreateTooManyRequestsResult(context.HttpContext);
            return;
        }

        await next();
    }
}
