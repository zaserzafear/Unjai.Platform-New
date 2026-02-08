namespace Unjai.Platform.Contracts.Models;

public sealed class AppResult<T>
{
    public bool Success { get; init; }
    public int HttpStatus { get; init; }
    public string StatusCode { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public long ServerUnixTime { get; init; }

    public static AppResult<T> Ok(
        int httpStatus = 200,
        string statusCode = "SUCCESS",
        string message = "",
        T? data = default)
        => new()
        {
            Success = true,
            HttpStatus = httpStatus,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            ServerUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };

    public static AppResult<T> Fail(
        int httpStatus,
        string statusCode = "",
        string message = "",
        T? data = default)
        => new()
        {
            Success = false,
            HttpStatus = httpStatus,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            ServerUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
}
