using Microsoft.Extensions.DependencyInjection;
using WorldCup.Application.Interfaces;
using WorldCup.Infrastructure.Repositories;

namespace WorldCup.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering repositories
/// </summary>
public static class RepositoryServiceExtensions
{
    /// <summary>
    /// Adds repository services to the DI container
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register specific repositories as needed
        services.AddScoped<ITeamRepository, TeamRepository>();
        
        // TODO: Add other repositories as we implement them

        return services;
    }
}
