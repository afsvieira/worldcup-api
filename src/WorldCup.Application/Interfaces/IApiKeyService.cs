using WorldCup.Application.DTOs.Common;
using WorldCup.Application.DTOs.Responses;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Service interface for API key management
/// </summary>
public interface IApiKeyService
{
    /// <summary>
    /// Get all API keys for a user
    /// </summary>
    Task<IEnumerable<ApiKeyDto>> GetApiKeysAsync(string userId);

    /// <summary>
    /// Create a new API key
    /// </summary>
    Task<ApiKeyCreateResult> CreateApiKeyAsync(string userId, string name);

    /// <summary>
    /// Revoke (deactivate) an API key
    /// </summary>
    Task<ServiceResult> RevokeApiKeyAsync(string userId, Guid apiKeyId);

    /// <summary>
    /// Delete an API key permanently
    /// </summary>
    Task<ServiceResult> DeleteApiKeyAsync(string userId, Guid apiKeyId);

    /// <summary>
    /// Get API key count for user
    /// </summary>
    Task<int> GetApiKeyCountAsync(string userId);
}
