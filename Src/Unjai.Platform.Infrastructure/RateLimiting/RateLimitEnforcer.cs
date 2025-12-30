using Microsoft.AspNetCore.Http;

namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed class RateLimitEnforcer
{
    private readonly RedisRateLimiter _rateLimiter;
    private readonly IRateLimitPolicyResolver _policyResolver;

    public RateLimitEnforcer(
        RedisRateLimiter rateLimiter,
        IRateLimitPolicyResolver policyResolver)
    {
        _rateLimiter = rateLimiter;
        _policyResolver = policyResolver;
    }

    public async Task<RateLimitResult> EnforceAsync(
        HttpContext httpContext,
        string policyName)
    {
        var policy = _policyResolver.Resolve(policyName);

        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rate-limit:{policy.Name}:{clientIp}";

        var allowed = await _rateLimiter.IsAllowedAsync(
            key,
            policy.Limit,
            policy.Window);

        return allowed
            ? RateLimitResult.Allowed()
            : RateLimitResult.Rejected(policy);
    }
}
