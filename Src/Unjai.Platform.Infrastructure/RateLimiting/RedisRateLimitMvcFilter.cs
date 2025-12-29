using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed class RedisRateLimitMvcFilter : IAsyncActionFilter
{
    private readonly RedisRateLimiter _limiter;
    private readonly IRateLimitPolicyResolver _resolver;
    private readonly string _policyName;

    public RedisRateLimitMvcFilter(
        RedisRateLimiter limiter,
        IRateLimitPolicyResolver resolver,
        string policyName)
    {
        _limiter = limiter;
        _resolver = resolver;
        _policyName = policyName;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var http = context.HttpContext;
        var policy = _resolver.Resolve(_policyName);

        var identity =
            http.User.Identity?.IsAuthenticated == true
                ? http.User.Identity!.Name!
                : http.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        var key = $"rate_limit:{policy.Name}:{identity}";

        var allowed = await _limiter.IsAllowedAsync(
            key,
            policy.Limit,
            policy.Window);

        if (!allowed)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status429TooManyRequests);
            return;
        }

        await next();
    }
}
