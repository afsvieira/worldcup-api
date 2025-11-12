using WorldCup.Application.DTOs.Common;
using WorldCup.Application.DTOs.Requests.Profile;
using WorldCup.Application.DTOs.Responses;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Service interface for user profile operations
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Get user profile information
    /// </summary>
    Task<ProfileDto?> GetProfileAsync(string userId);

    /// <summary>
    /// Update user profile
    /// </summary>
    Task<ServiceResult> UpdateProfileAsync(string userId, UpdateProfileRequest request);
}
