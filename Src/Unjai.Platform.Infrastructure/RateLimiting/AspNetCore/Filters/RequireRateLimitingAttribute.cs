using Microsoft.AspNetCore.Mvc;

namespace Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireRateLimitingAttribute : TypeFilterAttribute
{
    public string PolicyName { get; }

    public RequireRateLimitingAttribute(string policyName)
        : base(typeof(RedisRateLimitMvcFilter))
        => (PolicyName, Arguments) = (policyName, new object[] { policyName });
}
