namespace WorldCup.Application.DTOs.Common;

/// <summary>
/// Result for authentication operations
/// Extends ServiceResult with authentication-specific properties
/// </summary>
public class AuthResult : ServiceResult
{
    public string? UserId { get; set; }
    public bool RequiresEmailConfirmation { get; set; }

    public static AuthResult SuccessfulAuth(string? message = null, string? userId = null)
        => new() { Success = true, Message = message, UserId = userId };

    public static AuthResult FailedAuth(string message)
        => new() { Success = false, Message = message };

    public static AuthResult FailedAuth(IEnumerable<string> errors)
        => new() { Success = false, Errors = errors.ToList() };
}
