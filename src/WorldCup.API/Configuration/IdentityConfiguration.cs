using Microsoft.AspNetCore.Identity;
using WorldCup.Identity.Models;

namespace WorldCup.API.Configuration;

/// <summary>
/// Extension methods for configuring ASP.NET Core Identity
/// </summary>
public static class IdentityConfiguration
{
    /// <summary>
    /// Adds and configures ASP.NET Core Identity
    /// </summary>
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<WorldCup.Identity.Data.ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure cookie settings
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.LoginPath = "/account/login";
            options.AccessDeniedPath = "/account/login";
            options.SlidingExpiration = true;
        });

        return services;
    }
}
