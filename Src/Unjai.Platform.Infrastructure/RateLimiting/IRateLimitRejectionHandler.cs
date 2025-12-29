using Microsoft.AspNetCore.Http;

namespace Unjai.Platform.Infrastructure.RateLimiting;

public interface IRateLimitRejectionHandler
{
    ValueTask<IResult> HandleAsync(
        HttpContext httpContext,
        string policyName,
        CancellationToken cancellationToken);
}
