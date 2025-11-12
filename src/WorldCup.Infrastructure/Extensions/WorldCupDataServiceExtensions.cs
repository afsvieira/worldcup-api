using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorldCup.Infrastructure.Data;

namespace WorldCup.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering World Cup data services
/// </summary>
public static class WorldCupDataServiceExtensions
{
    /// <summary>
    /// Adds World Cup database context with read-only access
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration with connection string</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddWorldCupData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("WorldCupDataConnection")
            ?? throw new InvalidOperationException("WorldCupDataConnection connection string is not configured.");

        services.AddDbContext<WorldCupDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                
                sqlOptions.CommandTimeout(30);
            });

            // Disable change tracking for better read-only performance
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            // Enable sensitive data logging only in development
            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        return services;
    }
}
