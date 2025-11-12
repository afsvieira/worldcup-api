namespace WorldCup.Identity.Services;

/// <summary>
/// Service for managing API key generation and validation
/// </summary>
public interface IApiKeyService
{
    /// <summary>
    /// Generates a new API key for the specified user
    /// </summary>
    Task<string> GenerateApiKeyAsync(string entraExternalId, string name);
    
    /// <summary>
    /// Validates if an API key is active and valid
    /// </summary>
    Task<bool> ValidateApiKeyAsync(string apiKey);
    
    /// <summary>
    /// Gets the user ID associated with an API key
    /// </summary>
    Task<Guid?> GetUserIdByApiKeyAsync(string apiKey);
}