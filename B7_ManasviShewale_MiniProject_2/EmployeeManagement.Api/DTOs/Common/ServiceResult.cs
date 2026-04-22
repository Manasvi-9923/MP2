namespace EmployeeManagement.Api.DTOs.Common;

public class ServiceResult
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ServiceResult Ok(string message = "") => new()
    {
        Success = true,
        StatusCode = StatusCodes.Status200OK,
        Message = message
    };

    public static ServiceResult NotFound(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status404NotFound,
        Message = message
    };

    public static ServiceResult BadRequest(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status400BadRequest,
        Message = message
    };

    public static ServiceResult Conflict(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status409Conflict,
        Message = message
    };

    public static ServiceResult Unauthorized(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status401Unauthorized,
        Message = message
    };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Ok(T data, string message = "") => new()
    {
        Success = true,
        StatusCode = StatusCodes.Status200OK,
        Message = message,
        Data = data
    };

    public static ServiceResult<T> Created(T data, string message = "") => new()
    {
        Success = true,
        StatusCode = StatusCodes.Status201Created,
        Message = message,
        Data = data
    };

    public new static ServiceResult<T> BadRequest(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status400BadRequest,
        Message = message
    };

    public new static ServiceResult<T> NotFound(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status404NotFound,
        Message = message
    };

    public new static ServiceResult<T> Conflict(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status409Conflict,
        Message = message
    };

    public new static ServiceResult<T> Unauthorized(string message) => new()
    {
        Success = false,
        StatusCode = StatusCodes.Status401Unauthorized,
        Message = message
    };
}
