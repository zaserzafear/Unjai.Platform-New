using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Unjai.Platform.Infrastructure.RateLimiting.Abstractions;

public interface IMvcRateLimitResultFactory
{
    IActionResult CreateTooManyRequestsResult(HttpContext httpContext);
}
