using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WorldCup.Shared.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Sets the application startup time using the singleton service.
    /// </summary>
    /// <param name="app">WebApplication instance.</param>
    /// <param name="startupTime">Time when the application started.</param>
    /// <exception cref="ArgumentNullException">Thrown if 'app' is null.</exception>
    public static void SetStartupTime(this WebApplication app, DateTime startupTime)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        var service = app.Services.GetRequiredService<StartupTimeService>();
        service.Initialize(startupTime); // Uses the Initialize method to ensure uniqueness
    }

    /// <summary>
    /// Retrieves the application startup time from the singleton service.
    /// </summary>
    /// <param name="app">WebApplication instance.</param>
    /// <returns>Startup time, or fallback with log if not initialized.</returns>
    /// <exception cref="ArgumentNullException">Thrown if 'app' is null.</exception>
    public static DateTime GetStartupTime(this WebApplication app)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        var service = app.Services.GetRequiredService<StartupTimeService>();
        if (service.StartupTime == DateTime.MinValue)
        {
            var logger = app.Services.GetRequiredService<ILogger<StartupTimeService>>();
            logger.LogWarning("StartupTimeService was not initialized. Using current UTC as fallback.");
            return DateTime.UtcNow;
        }
        return service.StartupTime;
    }
}