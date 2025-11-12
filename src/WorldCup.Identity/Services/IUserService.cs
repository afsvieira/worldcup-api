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
    Task<User> RegisterUserAsync(string azureAdB2CId, string email, string name);
    
    /// <summary>
    /// Gets user by Azure AD B2C identifier
    /// </summary>
    Task<User?> GetUserByAzureIdAsync(string azureAdB2CId);
    
    /// <summary>
    /// Gets user by internal user ID
    /// </summary>
    Task<User?> GetUserByIdAsync(Guid userId);
}