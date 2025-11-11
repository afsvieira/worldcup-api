using Asp.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using WorldCup.Shared.Extensions;
using WorldCup.Shared.Middleware;

// Configure Serilog early in the bootstrap process
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting WorldCup API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "WorldCup.API")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithProperty("ApiVersion", context.Configuration["App:Version"] ?? "1.0.0"));

    // Add services to the container
    builder.Services.AddControllers();

    // Add startup time tracking service
    builder.Services.AddSingleton<StartupTimeService>();

    // Add API versioning
    builder.Services.AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader()
        );
    }).AddMvc();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WorldCup API",
        Description = "A comprehensive API for managing and accessing FIFA World Cup data, including matches, teams, players, and statistics.",
        Contact = new OpenApiContact
        {
            Name = "WorldCup API Team",
            Email = "afsvieira@afsvieira.com",
            Url = new Uri("https://github.com/afsvieira/worldcup-api")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments for better documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);

    // Enable annotations for additional metadata
    options.EnableAnnotations();

    // Order actions by relative path
    options.OrderActionsBy(apiDesc => $"{apiDesc.RelativePath}");
});

    // Register health checks service
    builder.Services.AddHealthChecks()
        // Add a check for the API's environment (non-critical)
        .AddCheck("Environment", () =>
        {
            var env = builder.Environment;
            return HealthCheckResult.Healthy($"Environment: {env.EnvironmentName}");
        })
        // Add uptime check (will be calculated at runtime)
        .AddCheck("Uptime", () => HealthCheckResult.Healthy("Uptime will be calculated at runtime"));

    var app = builder.Build();

    // Set the startup time for later use in health checks
    app.SetStartupTime(DateTime.UtcNow);

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
            if (app.Environment.IsDevelopment())
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

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.MapControllers();

    Log.Information("WorldCup API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
