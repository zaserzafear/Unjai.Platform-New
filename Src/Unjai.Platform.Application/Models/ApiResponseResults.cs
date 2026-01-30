using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Unjai.Platform.Application.Models;

public static class ApiResponseResults
{
    public static IResult ToHttpResult<T>(AppResult<T> result)
    {
        var response = new ApiResponse<T>
        {
            Success = result.Success,
            HttpStatus = result.HttpStatus,
            StatusCode = result.StatusCode,
            Message = result.Message,
            TraceId = Activity.Current?.TraceId.ToString(),
            Data = result.Data,
        };

        return TypedResults.Json(
            response,
            statusCode: result.HttpStatus
        );
    }
}
