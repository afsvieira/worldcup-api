using Serilog;
using WorldCup.API.Configuration;
using WorldCup.Shared.Extensions;

// Configure Serilog early in the bootstrap process
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting WorldCup API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.ConfigureSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddControllersWithViews();

    // Configure application services
    builder.Services.AddDatabaseConfiguration(builder.Configuration);
    builder.Services.AddIdentityConfiguration();
    builder.Services.AddEmailConfiguration(builder.Configuration);
    builder.Services.AddApplicationServices(); // Business logic services
    builder.Services.AddVersioningConfiguration();
    builder.Services.AddSwaggerConfiguration();
    builder.Services.AddHealthCheckConfiguration(builder.Environment);

    var app = builder.Build();

    // Set the startup time for later use in health checks
    app.SetStartupTime(DateTime.UtcNow);

    // Configure middleware pipeline
    app.UseLoggingConfiguration(app.Environment);
    app.UseSwaggerConfiguration(app.Environment);

    app.UseHttpsRedirection();
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
