using WorldCup.Domain.Entities;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Repository for Team entities
/// </summary>
public interface ITeamRepository : IRepository<Team>
{
    /// <summary>
    /// Searches teams by name (partial match)
    /// </summary>
    Task<IReadOnlyList<Team>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams by confederation
    /// </summary>
    Task<IReadOnlyList<Team>> GetByConfederationAsync(string confederation, CancellationToken cancellationToken = default);
}
