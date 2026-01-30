using Microsoft.AspNetCore.Http;
using Unjai.Platform.Infrastructure.RateLimiting.Context;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Delegates;

public sealed class RateLimitContextHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RateLimitContextHandler(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null && httpContext.TryGetRateLimitContext(out var ctx))
        {
            request.Headers.Add(
                RateLimitHeaders.Context,
                ctx!.Signature);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
