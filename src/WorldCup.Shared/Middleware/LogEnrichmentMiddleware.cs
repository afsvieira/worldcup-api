using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog.Context;
using System.Diagnostics;

namespace WorldCup.Shared.Middleware;

/// <summary>
/// Middleware to enrich logs with contextual information such as RequestId, CorrelationId, and TraceId.
/// </summary>
public class LogEnrichmentMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public LogEnrichmentMiddleware(RequestDelegate next, IHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get or generate correlation ID
        var correlationId = GetOrGenerateCorrelationId(context);

        // Get request ID (ASP.NET Core automatically generates this)
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Add correlation ID to response headers for traceability
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
            {
                context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);
            }
            return Task.CompletedTask;
        });

        // Essential properties for all environments
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestPath", context.Request.Path.Value))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        {
            // Additional verbose properties only in Development
            if (_environment.IsDevelopment())
            {
                var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
                var spanId = Activity.Current?.SpanId.ToString() ?? string.Empty;
                var clientIp = GetClientIpAddress(context);
                var userAgent = context.Request.Headers.UserAgent.ToString();

                using (LogContext.PushProperty("TraceId", traceId))
                using (LogContext.PushProperty("SpanId", spanId))
                using (LogContext.PushProperty("ClientIp", clientIp ?? "unknown"))
                using (LogContext.PushProperty("UserAgent", userAgent))
                {
                    await _next(context);
                }
            }
            else
            {
                // Production: minimal properties
                await _next(context);
            }
        }
    }

    private static string GetOrGenerateCorrelationId(HttpContext context)
    {
        // Check if correlation ID exists in request headers
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId) 
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        // Generate a new correlation ID
        return Guid.NewGuid().ToString();
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP (when behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Fallback to remote IP address
        return context.Connection.RemoteIpAddress?.ToString();
    }
}
