using Microsoft.AspNetCore.Mvc;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;

namespace Unjai.Platform.Mvc.CustomerUser.RateLimiting;

internal sealed class MvcRateLimitResultFactory
    : IMvcRateLimitResultFactory
{
    public IActionResult CreateTooManyRequestsResult(HttpContext context)
    {
        context.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        return new ViewResult
        {
            ViewName = "TooManyRequests" // /Views/Shared/TooManyRequests.cshtml
        };
    }
}
