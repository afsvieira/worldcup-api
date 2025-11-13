using Microsoft.Extensions.DependencyInjection;
using WorldCup.Application.Interfaces;
using WorldCup.Application.Services;

namespace WorldCup.Application.Extensions;

/// <summary>
/// Service registration extensions for application layer.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers WorldCup application layer services (DTOs, business logic).
    /// </summary>
    public static IServiceCollection AddWorldCupApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IStadiumService, StadiumService>();
        
        return services;
    }
}
