using WorldCup.Application.Interfaces;
using WorldCup.Identity.Services;

namespace WorldCup.API.Configuration;

/// <summary>
/// Configuration for application services
/// </summary>
public static class ApplicationServicesConfiguration
{
    /// <summary>
    /// Adds application services to the DI container
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register business logic services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        
        return services;
    }
}
