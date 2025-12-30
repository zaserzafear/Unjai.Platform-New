using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed class RedisRateLimitFilter : IEndpointFilter
{
    private readonly RedisRateLimiter _rateLimiter;
    private readonly IRateLimitPolicyResolver _policyResolver;
    private readonly string _policyName;

    public RedisRateLimitFilter(
        RedisRateLimiter rateLimiter,
        IRateLimitPolicyResolver policyResolver,
        string policyName)
    {
        _rateLimiter = rateLimiter;
        _policyResolver = policyResolver;
        _policyName = policyName;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var policy = _policyResolver.Resolve(_policyName);

        var httpContext = context.HttpContext;

        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rate-limit:{policy.Name}:{clientIp}";

        var allowed = await _rateLimiter.IsAllowedAsync(
            key,
            policy.Limit,
            policy.Window);

        if (!allowed)
        {
            return Results.StatusCode(StatusCodes.Status429TooManyRequests);
        }

        return await next(context);
    }
}
