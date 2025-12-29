using Microsoft.AspNetCore.Http;

namespace Unjai.Platform.Infrastructure.RateLimiting;

public sealed class RedisRateLimitFilter : IEndpointFilter
{
    private readonly RedisRateLimiter _limiter;
    private readonly IRateLimitPolicyResolver _resolver;
    private readonly IRateLimitRejectionHandler _rejectionHandler;
    private readonly string _policy;

    public RedisRateLimitFilter(
        RedisRateLimiter limiter,
        IRateLimitPolicyResolver resolver,
        IRateLimitRejectionHandler rejectionHandler,
        string policy)
    {
        _limiter = limiter;
        _resolver = resolver;
        _rejectionHandler = rejectionHandler;
        _policy = policy;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var policy = _resolver.Resolve(_policy);
        var key = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var allowed = await _limiter.IsAllowedAsync(key, policy.Limit, policy.Window);

        if (!allowed)
        {
            return await _rejectionHandler.HandleAsync(
                context.HttpContext,
                _policy,
                context.HttpContext.RequestAborted);
        }

        return await next(context);
    }
}
