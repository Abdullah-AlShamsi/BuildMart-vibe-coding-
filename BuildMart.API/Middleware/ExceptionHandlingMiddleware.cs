using System.Net;
using System.Text.Json;
using BuildMart.Application.DTOs.Common;
using BuildMart.Domain.Exceptions;

namespace BuildMart.API.Middleware;

/// <summary>
/// Catches every unhandled exception, logs it, and returns a uniform
/// ApiResponse&lt;object&gt; envelope with the correct HTTP status code —
/// so controllers never need their own try/catch blocks for these cases.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            BadRequestException => (HttpStatusCode.BadRequest, exception.Message),
            ConflictException => (HttpStatusCode.Conflict, exception.Message),
            ForbiddenException => (HttpStatusCode.Forbidden, exception.Message),
            UnauthorizedAppException => (HttpStatusCode.Unauthorized, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("{ExceptionType}: {Message}", exception.GetType().Name, exception.Message);
        }

        var response = ApiResponse<object>.FailureResponse(
            message,
            _environment.IsDevelopment() && statusCode == HttpStatusCode.InternalServerError
                ? new List<string> { exception.ToString() }
                : null);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
