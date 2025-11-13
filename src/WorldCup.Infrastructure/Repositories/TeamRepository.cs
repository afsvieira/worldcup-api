using Microsoft.EntityFrameworkCore;
using WorldCup.Application.Interfaces;
using WorldCup.Domain.Entities;
using WorldCup.Infrastructure.Data;

namespace WorldCup.Infrastructure.Repositories;

/// <summary>
/// Team repository implementation
/// </summary>
public class TeamRepository : Repository<Team>, ITeamRepository
{
    public TeamRepository(WorldCupDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Team>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Name != null && t.Name.Contains(name))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Team>> GetByConfederationAsync(string confederation, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Confederation == confederation)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }
}
