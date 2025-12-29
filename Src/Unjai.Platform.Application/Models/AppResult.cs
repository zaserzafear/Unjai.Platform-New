namespace Unjai.Platform.Application.Models;

public sealed class AppResult<T>
{
    public bool Success { get; init; }
    public int HttpStatus { get; init; }
    public string StatusCode { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public long ServerUnixTime { get; init; }

    public static AppResult<T> Ok(T data, string statusCode = "SUCCESS", string message = "")
        => new()
        {
            Success = true,
            HttpStatus = 200,
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
