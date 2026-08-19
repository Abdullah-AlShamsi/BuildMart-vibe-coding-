namespace BuildMart.Application.DTOs.Common;

/// <summary>
/// Uniform envelope for every API response so the frontend can rely on
/// one shape regardless of endpoint: { success, message, data, errors }.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public T? Data { get; set; }

    public List<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}
