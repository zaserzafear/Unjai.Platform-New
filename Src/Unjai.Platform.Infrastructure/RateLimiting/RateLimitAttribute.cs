using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Unjai.Platform.Infrastructure.RateLimiting;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RateLimitAttribute(string policy) : Attribute, IFilterFactory
{
    public string Policy { get; } = policy;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return new RedisRateLimitMvcFilter(
            serviceProvider.GetRequiredService<RedisRateLimiter>(),
            serviceProvider.GetRequiredService<IRateLimitPolicyResolver>(),
            Policy);
    }

    public bool IsReusable => false;
}
