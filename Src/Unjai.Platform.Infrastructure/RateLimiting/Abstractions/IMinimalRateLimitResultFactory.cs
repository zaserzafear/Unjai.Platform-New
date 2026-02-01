using Microsoft.AspNetCore.Http;

namespace Unjai.Platform.Infrastructure.RateLimiting.Abstractions;

public interface IMinimalRateLimitResultFactory
{
    IResult CreateTooManyRequestsResult(HttpContext httpContext);
}
