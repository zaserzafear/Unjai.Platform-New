using Unjai.Platform.Infrastructure.RateLimiting.Core;

namespace Unjai.Platform.Infrastructure.RateLimiting.Abstractions;

internal interface IRateLimitPolicyResolver
{
    RateLimitPolicy Resolve(string policyName);
}
