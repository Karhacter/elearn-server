namespace elearn_server.Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }
    public object? Meta { get; init; }

    public static ServiceResult<T> Ok(T? data, string message = "Success.", object? meta = null) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, Message = message, Data = data, Meta = meta };

    public static ServiceResult<T> Created(T? data, string message = "Created.", object? meta = null) =>
        new() { Success = true, StatusCode = StatusCodes.Status201Created, Message = message, Data = data, Meta = meta };

    public static ServiceResult<T> Fail(int statusCode, string message, Dictionary<string, string[]>? errors = null) =>
        new() { Success = false, StatusCode = statusCode, Message = message, Errors = errors };
}
