namespace Unjai.Platform.Contracts.Models;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public int HttpStatus { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public long ServerUnixTime => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
