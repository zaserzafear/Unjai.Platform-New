using System.Diagnostics;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Infrastructure.RateLimiting;

namespace Unjai.Platform.Api.RateLimiting;

public sealed class ApiRateLimitRejectionHandler
    : IRateLimitRejectionHandler
{
    public ValueTask<IResult> HandleAsync(
        HttpContext httpContext,
        string policyName,
        CancellationToken cancellationToken)
    {
        var response = new ApiResponse<object?>
        {
            Success = false,
            HttpStatus = StatusCodes.Status429TooManyRequests,
            StatusCode = "RATE_LIMIT_EXCEEDED",
            Message = $"Rate limit exceeded ({policyName})",
            TraceId = Activity.Current?.TraceId.ToString(),
            Data = null
        };

        return ValueTask.FromResult<IResult>(
            TypedResults.Json(
                response,
                statusCode: StatusCodes.Status429TooManyRequests));
    }
}
