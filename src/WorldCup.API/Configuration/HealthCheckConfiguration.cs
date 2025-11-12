using Microsoft.Extensions.Diagnostics.HealthChecks;
using WorldCup.Shared.Extensions;

namespace WorldCup.API.Configuration;

/// <summary>
/// Extension methods for configuring health checks
/// </summary>
public static class HealthCheckConfiguration
{
    /// <summary>
    /// Adds and configures health checks
    /// </summary>
    public static IServiceCollection AddHealthCheckConfiguration(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        // Add startup time tracking service
        services.AddSingleton<StartupTimeService>();

        // Register health checks
        services.AddHealthChecks()
            .AddCheck("Environment", () =>
            {
                return HealthCheckResult.Healthy($"Environment: {environment.EnvironmentName}");
            })
            .AddCheck("Uptime", () =>
            {
                return HealthCheckResult.Healthy("Uptime will be calculated at runtime");
            });

        return services;
    }
}
