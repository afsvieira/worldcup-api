using WorldCup.Application.DTOs.Common;
using WorldCup.Application.DTOs.Requests.Auth;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<AuthResult> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticate user and sign in
    /// </summary>
    Task<AuthResult> LoginAsync(LoginRequest request);

    /// <summary>
    /// Sign out current user
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// Confirm user email with token
    /// </summary>
    Task<AuthResult> ConfirmEmailAsync(string userId, string token);

    /// <summary>
    /// Resend email confirmation
    /// </summary>
    Task<AuthResult> ResendConfirmationEmailAsync(string userId, string baseUrl);

    /// <summary>
    /// Change user password
    /// </summary>
    Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
