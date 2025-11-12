using WorldCup.Domain.Entities;

namespace WorldCup.Identity.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = new();

    public Task<User> RegisterUserAsync(string azureAdB2CId, string email, string name)
    {
        var existingUser = _users.FirstOrDefault(u => u.AzureAdB2CId == azureAdB2CId);
        if (existingUser != null)
            return Task.FromResult(existingUser);

        var user = new User
        {
            Id = Guid.NewGuid(),
            AzureAdB2CId = azureAdB2CId,
            Email = email,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByAzureIdAsync(string azureAdB2CId)
    {
        var user = _users.FirstOrDefault(u => u.AzureAdB2CId == azureAdB2CId);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByIdAsync(Guid userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        return Task.FromResult(user);
    }
}