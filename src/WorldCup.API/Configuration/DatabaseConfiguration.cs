using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorldCup.Identity.Data;
using WorldCup.Identity.Models;

namespace WorldCup.API.Configuration;

/// <summary>
/// Extension methods for configuring database and Entity Framework
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Adds database context and configures Entity Framework
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context for Identity
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
