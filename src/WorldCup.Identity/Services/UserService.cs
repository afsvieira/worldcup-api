using WorldCup.Domain.Entities;

namespace WorldCup.Identity.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = new();

    public Task<User> RegisterUserAsync(string entraExternalId, string email, string name)
    {
        var existingUser = _users.FirstOrDefault(u => u.EntraExternalId == entraExternalId);
        if (existingUser != null)
            return Task.FromResult(existingUser);

        var user = new User
        {
            Id = Guid.NewGuid(),
            EntraExternalId = entraExternalId,
            Email = email,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByExternalIdAsync(string entraExternalId)
    {
        var user = _users.FirstOrDefault(u => u.EntraExternalId == entraExternalId);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByIdAsync(Guid userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        return Task.FromResult(user);
    }
}