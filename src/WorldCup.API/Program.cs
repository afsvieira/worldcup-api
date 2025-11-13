using Serilog;
using WorldCup.API.Configuration;
using WorldCup.Application.Extensions;
using WorldCup.Shared.Extensions;
using WorldCup.Infrastructure.Extensions;

// Configure Serilog early in the bootstrap process
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting WorldCup API...");

try
{
    // Disable browser link and hot reload middleware completely
    Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "");
    Environment.SetEnvironmentVariable("ASPNETCORE_AUTO_RELOAD_WS_ENDPOINT", "");
    Environment.SetEnvironmentVariable("DOTNET_WATCH", "false");
    
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.ConfigureSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    
    // Add MVC with views but disable browser link/hot reload
    var mvcBuilder = builder.Services.AddControllersWithViews();
    
    // Only add runtime compilation in Development if explicitly needed
    // (Disabled to prevent browser refresh infinite loop)
    // if (builder.Environment.IsDevelopment())
    // {
    //     mvcBuilder.AddRazorRuntimeCompilation();
    // }

    // Configure application services
    builder.Services.AddDatabaseConfiguration(builder.Configuration);
    builder.Services.AddIdentityConfiguration();
    
    // Add distributed cache (using Memory Cache for now - can switch to Redis later)
    builder.Services.AddDistributedMemoryCache();
    // TODO: Switch to Redis when ready:
    // builder.Services.AddStackExchangeRedisCache(options =>
    // {
    //     options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    // });
    
    builder.Services.AddWorldCupData(builder.Configuration); // World Cup read-only database
    builder.Services.AddRepositories(); // Register repositories
    builder.Services.AddEmailConfiguration(builder.Configuration);
    builder.Services.AddWorldCupApplicationServices(); // WorldCup business logic services
    builder.Services.AddApplicationServices(); // Auth/Profile/ApiKey services
    builder.Services.AddResponseCaching(); // Add response caching middleware support
    builder.Services.AddVersioningConfiguration();
    builder.Services.AddSwaggerConfiguration();
    builder.Services.AddHealthCheckConfiguration(builder.Environment);

    var app = builder.Build();

    // Set the startup time for later use in health checks
    app.SetStartupTime(DateTime.UtcNow);

    // Configure middleware pipeline
    app.UseLoggingConfiguration(app.Environment);
    app.UseSwaggerConfiguration(app.Environment);

    // Block all hot reload related requests and add no-cache headers in development
    app.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (path != null && (path.Contains("aspnetcore-browser-refresh") || 
                           path.Contains("_framework/aspnetcore") ||
                           path.Contains("_vs/browserLink")))
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Not Found");
            return;
        }
        
        // Add no-cache headers in development
        if (app.Environment.IsDevelopment())
        {
            context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.Headers.Add("Pragma", "no-cache");
            context.Response.Headers.Add("Expires", "0");
        }
        
        await next();
    });

    app.UseHttpsRedirection();
    app.UseResponseCaching(); // Enable response caching middleware
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

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
