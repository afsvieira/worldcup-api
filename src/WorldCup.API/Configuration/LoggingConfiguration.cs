using Serilog;
using Serilog.Events;
using WorldCup.Shared.Extensions;
using WorldCup.Shared.Middleware;

namespace WorldCup.API.Configuration;

/// <summary>
/// Extension methods for configuring logging with Serilog
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Configures Serilog as the logging provider
    /// </summary>
    public static void ConfigureSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "WorldCup.API")
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("ApiVersion", context.Configuration["App:Version"] ?? "1.0.0"));
    }

    /// <summary>
    /// Configures Serilog request logging middleware
    /// </summary>
    public static IApplicationBuilder UseLoggingConfiguration(
        this IApplicationBuilder app,
        IWebHostEnvironment environment)
    {
        // Add Serilog request logging with appropriate log levels based on status codes
        app.UseSerilogRequestLogging(options =>
        {
            // Customize log level based on response status code
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null)
                    return LogEventLevel.Error;

                var statusCode = httpContext.Response.StatusCode;

                // 5xx - Server errors: Error
                if (statusCode >= 500)
                    return LogEventLevel.Error;

                // 4xx - Client errors: Warning (except 401, 404 which are common)
                if (statusCode >= 400)
                {
                    if (statusCode == 401 || statusCode == 404)
                        return LogEventLevel.Information;

                    return LogEventLevel.Warning;
                }

                // 2xx, 3xx - Success: Information (or Debug for health checks)
                if (httpContext.Request.Path.StartsWithSegments("/api/v1/health") ||
                    httpContext.Request.Path.StartsWithSegments("/health"))
                    return LogEventLevel.Debug;

                return LogEventLevel.Information;
            };

            // Enrich with additional context
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);

                // Only include detailed info in Development
                if (environment.IsDevelopment())
                {
                    diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString());
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                }
            };

            // Customize message template
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        });

        // Add global exception handling middleware (must be early in pipeline)
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        // Add log enrichment middleware (adds CorrelationId, RequestId, etc.)
        app.UseMiddleware<LogEnrichmentMiddleware>();

        return app;
    }
}
