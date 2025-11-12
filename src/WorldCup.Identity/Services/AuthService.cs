using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WorldCup.Application.DTOs.Common;
using WorldCup.Application.DTOs.Requests.Auth;
using WorldCup.Application.Interfaces;
using WorldCup.Identity.Models;
using WorldCup.Domain.Enums;
using WorldCup.Shared.Services;
using System.Web;

namespace WorldCup.Identity.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly EmailRateLimiter _emailRateLimiter;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        EmailRateLimiter emailRateLimiter,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _emailRateLimiter = emailRateLimiter;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PlanType = PlanType.Free,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return AuthResult.FailedAuth(result.Errors.Select(e => e.Description));
        }

        _logger.LogInformation("User {Email} registered successfully", request.Email);

        // Auto sign-in
        await _signInManager.SignInAsync(user, isPersistent: false);

        var authResult = AuthResult.SuccessfulAuth(
            "Account created successfully! Please check your email to verify your account.",
            user.Id
        );
        authResult.RequiresEmailConfirmation = true;
        return authResult;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
            return AuthResult.FailedAuth("Invalid email or password.");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName ?? string.Empty,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
            return AuthResult.FailedAuth("Invalid email or password.");
        }

        _logger.LogInformation("User {Email} logged in successfully", request.Email);
        return AuthResult.SuccessfulAuth("Logged in successfully.", user.Id);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
    }

    public async Task<AuthResult> ConfirmEmailAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return AuthResult.FailedAuth("Invalid confirmation link.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return AuthResult.FailedAuth("User not found.");
        }

        if (user.EmailConfirmed)
        {
            return AuthResult.SuccessfulAuth("Email already confirmed.");
        }

        var decodedToken = HttpUtility.UrlDecode(token);
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Email confirmation failed for user {UserId}", userId);
            return AuthResult.FailedAuth("Email confirmation failed. The link may be expired.");
        }

        _logger.LogInformation("Email confirmed for user {Email}", user.Email);
        return AuthResult.SuccessfulAuth("Email confirmed successfully!");
    }

    public async Task<AuthResult> ResendConfirmationEmailAsync(string userId, string baseUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return AuthResult.FailedAuth("User not found.");
        }

        if (user.EmailConfirmed)
        {
            return AuthResult.FailedAuth("Email is already confirmed.");
        }

        // Check rate limiting
        if (!_emailRateLimiter.CanSendEmail(userId, out var remainingTime))
        {
            var minutes = Math.Ceiling(remainingTime!.Value.TotalSeconds / 60.0);
            return AuthResult.FailedAuth($"Please wait {minutes} minute(s) before requesting another confirmation email.");
        }

        // Generate new token and send email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var confirmationLink = $"{baseUrl}/account/confirm-email?userId={userId}&token={encodedToken}";

        try
        {
            var emailBody = EmailTemplates.GenerateEmailConfirmationTemplate(
                user.FirstName,
                confirmationLink);

            await _emailSender.SendEmailAsync(
                user.Email ?? string.Empty,
                "Confirm Your Email - World Cup API",
                emailBody);

            _emailRateLimiter.RecordEmailSent(userId);
            _logger.LogInformation("Confirmation email resent to {Email}", user.Email);

            return AuthResult.SuccessfulAuth("Confirmation email sent! Please check your inbox.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            return AuthResult.FailedAuth("Failed to send confirmation email. Please try again later.");
        }
    }

    public async Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return AuthResult.FailedAuth("User not found.");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Password change failed for user {UserId}", userId);
            return AuthResult.FailedAuth(result.Errors.Select(e => e.Description));
        }

        // Refresh sign-in to update security stamp
        await _signInManager.RefreshSignInAsync(user);

        _logger.LogInformation("Password changed successfully for user {Email}", user.Email);
        return AuthResult.SuccessfulAuth("Password changed successfully!");
    }
}
