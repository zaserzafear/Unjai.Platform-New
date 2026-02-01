using Microsoft.AspNetCore.Http;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Context;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;
using Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Redis;

namespace Unjai.Platform.Infrastructure.RateLimiting.Core;

internal sealed class RateLimitEnforcer(
    RedisRateLimiter rateLimiter,
    IRateLimitPolicyResolver policyResolver,
    IRateLimitContextSigner signer
    )
{
    public async Task<RateLimitResult> EnforceAsync(
        HttpContext httpContext,
        string policyName)
    {
        // 1. Validate signed context (internal call)
        if (httpContext.Request.Headers.TryGetValue(
                RateLimitHeaders.Context, out var header) &&
            signer.TryValidate(header!, policyName))
        {
            return RateLimitResult.Allowed();
        }

        // 2. Enforce Redis
        var policy = policyResolver.Resolve(policyName);
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var key = $"rate-limit:{policy.Name}:{clientIp}";

        var allowed = await rateLimiter.IsAllowedAsync(
            key,
            policy.Strategy,
            policy.Limit,
            policy.Window,
            policy.TokensPerPeriod,
            policy.ReplenishmentPeriod);

        if (!allowed)
            return RateLimitResult.Rejected(policy);

        // 3. Attach signed context for downstream
        var signedContext = new RateLimitContext(
            policy.Name,
            signer.Sign(policyName),
            DateTimeOffset.UtcNow);
        httpContext.SetRateLimitContext(signedContext);

        return RateLimitResult.Allowed();
    }
}
