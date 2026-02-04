using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Unjai.Platform.Infrastructure.Security.Auth.Delegates;

internal sealed class HealthChecksApiKeyHandler : AuthorizationHandler<HealthChecksApiKeyRequirement>
{
    private readonly string _apiKey;

    public HealthChecksApiKeyHandler(string apiKey)
    {
        _apiKey = apiKey;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HealthChecksApiKeyRequirement requirement)
    {
        var httpContext = (context.Resource as DefaultHttpContext)!;

        if (!httpContext.Request.Headers.TryGetValue("X-API-KEY", out var key))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (key == _apiKey)
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}
