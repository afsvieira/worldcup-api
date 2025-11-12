using WorldCup.Domain.Entities;

namespace WorldCup.Identity.Services;

/// <summary>
/// Service for managing user registration and retrieval
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user or returns existing user
    /// </summary>
    Task<User> RegisterUserAsync(string entraExternalId, string email, string name);
    
    /// <summary>
    /// Gets user by Entra External ID identifier
    /// </summary>
    Task<User?> GetUserByExternalIdAsync(string entraExternalId);
    
    /// <summary>
    /// Gets user by internal user ID
    /// </summary>
    Task<User?> GetUserByIdAsync(Guid userId);
}