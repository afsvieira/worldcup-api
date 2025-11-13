using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace WorldCup.Shared.Middleware;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions and returns appropriate responses.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
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
        // Log the exception with ERROR level and full stack trace
        _logger.LogError(
            exception,
            "Unhandled exception occurred while processing request {RequestMethod} {RequestPath}. ExceptionType: {ExceptionType}",
            context.Request.Method,
            context.Request.Path,
            exception.GetType().Name);

        // Prepare error response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorResponse = new
        {
            error = new
            {
                code = "INTERNAL_ERROR",
                message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred. Please try again later or contact support if the problem persists.",
                statusCode = context.Response.StatusCode,
                details = _environment.IsDevelopment() 
                    ? new[]
                    {
                        new
                        {
                            field = "exception",
                            issue = exception.GetType().Name
                        }
                    }
                    : null,
                // Only expose technical details in Development
                stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null,
                innerException = _environment.IsDevelopment() ? exception.InnerException?.Message : null
            }
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment(),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
