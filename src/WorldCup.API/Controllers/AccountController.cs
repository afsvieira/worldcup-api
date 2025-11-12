using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldCup.API.Models;
using WorldCup.Application.Interfaces;
using WorldCup.Application.DTOs.Requests.Auth;
using WorldCup.Domain.Enums;

namespace WorldCup.API.Controllers;

/// <summary>
/// Controller for user account management
/// Follows Clean Architecture principles - orchestrates services, doesn't contain business logic
/// </summary>
public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly IApiKeyService _apiKeyService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthService authService,
        IProfileService profileService,
        IApiKeyService apiKeyService,
        ILogger<AccountController> logger)
    {
        _authService = authService;
        _profileService = profileService;
        _apiKeyService = apiKeyService;
        _logger = logger;
    }

    #region Authentication

    /// <summary>
    /// Display registration page
    /// </summary>
    [HttpGet("/account/register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Profile));
        }
        return View();
    }

    /// <summary>
    /// Process user registration
    /// </summary>
    [HttpPost("/account/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new RegisterRequest
        {
            Email = model.Email,
            Password = model.Password,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _authService.RegisterAsync(request);

        if (result.Success)
        {
            // Send confirmation email asynchronously (don't wait)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            _ = _authService.ResendConfirmationEmailAsync(result.UserId!, baseUrl);

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Profile));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    /// <summary>
    /// Display login page
    /// </summary>
    [HttpGet("/account/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Profile));
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Process user login
    /// </summary>
    [HttpPost("/account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new LoginRequest
        {
            Email = model.Email,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _authService.LoginAsync(request);

        if (result.Success)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Profile));
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Invalid login attempt.");
        return View(model);
    }

    /// <summary>
    /// Process user logout
    /// </summary>
    [HttpPost("/account/logout")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Confirm user email address
    /// </summary>
    [HttpGet("/account/confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var result = await _authService.ConfirmEmailAsync(userId, token);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;

            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Profile));
            }
            return RedirectToAction(nameof(Login));
        }

        TempData["ErrorMessage"] = result.Message;
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Resend email confirmation
    /// </summary>
    [HttpPost("/account/resend-confirmation")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ResendConfirmationEmail()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = await _authService.ResendConfirmationEmailAsync(userId, baseUrl);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Profile));
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("/account/change-password")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return RedirectToAction(nameof(Profile));
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var result = await _authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Profile));
    }

    #endregion

    #region Profile

    /// <summary>
    /// Display user profile
    /// </summary>
    [HttpGet("/account/profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var model = new ProfileViewModel
        {
            Username = profile.Email,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            MemberSince = profile.CreatedAt,
            EmailConfirmed = profile.EmailConfirmed,
            PlanType = profile.PlanType,
            RequestsToday = profile.MonthlyRequests, // TODO: Implement daily tracking
            RequestsThisWeek = profile.MonthlyRequests, // TODO: Implement weekly tracking
            RequestsThisMonth = profile.MonthlyRequests,
            DailyLimit = GetDailyLimitForPlan(profile.PlanType),
            ApiKeyCount = profile.ApiKeyCount,
            MaxApiKeys = profile.MaxApiKeys
        };

        return View(model);
    }

    #endregion

    #region API Keys

    /// <summary>
    /// Display API keys management page
    /// </summary>
    [HttpGet("/account/apikeys")]
    [Authorize]
    public async Task<IActionResult> ApiKeys()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var apiKeys = await _apiKeyService.GetApiKeysAsync(userId);

        var model = new ApiKeysViewModel
        {
            Username = profile.Email,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            PlanType = profile.PlanType,
            MaxApiKeys = profile.MaxApiKeys,
            MaxMonthlyRequests = profile.MaxMonthlyRequests,
            MemberSince = profile.CreatedAt,
            ApiKeys = apiKeys.Select(k => new ApiKeyItemViewModel
            {
                Id = k.Id.ToString(),
                Name = k.Name,
                KeyPreview = k.KeyPreview,
                IsActive = k.IsActive,
                CreatedAt = k.CreatedAt,
                LastUsedAt = null // TODO: Implement usage tracking
            }).ToList(),
            NewlyCreatedApiKey = TempData["NewApiKey"] as string,
            NewlyCreatedKeyName = TempData["NewApiKeyName"] as string
        };

        return View(model);
    }

    /// <summary>
    /// Create a new API key
    /// </summary>
    [HttpPost("/account/apikeys/create")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> CreateApiKey(string name)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["ErrorMessage"] = "API key name is required.";
            return RedirectToAction(nameof(ApiKeys));
        }

        var result = await _apiKeyService.CreateApiKeyAsync(userId, name.Trim());

        if (result.Success)
        {
            // Pass the plain text key via TempData - shown ONLY ONCE
            TempData["NewApiKey"] = result.PlainKey;
            TempData["NewApiKeyName"] = name;
            TempData["SuccessMessage"] = $"API key '{name}' created successfully! Make sure to copy it now - you won't be able to see it again.";
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(ApiKeys));
    }

    /// <summary>
    /// Revoke an API key
    /// </summary>
    [HttpPost("/account/apikeys/revoke")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> RevokeApiKey(string apiKeyId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        if (!Guid.TryParse(apiKeyId, out var keyGuid))
        {
            TempData["ErrorMessage"] = "Invalid API key ID.";
            return RedirectToAction(nameof(ApiKeys));
        }

        var result = await _apiKeyService.RevokeApiKeyAsync(userId, keyGuid);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(ApiKeys));
    }

    /// <summary>
    /// Delete an API key
    /// </summary>
    [HttpPost("/account/apikeys/delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteApiKey(string apiKeyId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        if (!Guid.TryParse(apiKeyId, out var keyGuid))
        {
            TempData["ErrorMessage"] = "Invalid API key ID.";
            return RedirectToAction(nameof(ApiKeys));
        }

        var result = await _apiKeyService.DeleteApiKeyAsync(userId, keyGuid);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(ApiKeys));
    }

    #endregion

    #region Security & Settings

    /// <summary>
    /// Display security settings page
    /// </summary>
    [HttpGet("/account/security")]
    [Authorize]
    public async Task<IActionResult> Security()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var profileDto = await _profileService.GetProfileAsync(userId);
        if (profileDto == null)
        {
            return NotFound();
        }

        var model = new AccountLayoutViewModel
        {
            Email = profileDto.Email,
            FirstName = profileDto.FirstName,
            LastName = profileDto.LastName,
            PlanType = profileDto.PlanType.ToString(),
            ApiKeyCount = profileDto.ApiKeyCount,
            MaxApiKeys = profileDto.MaxApiKeys,
            MemberSince = profileDto.CreatedAt
        };

        return View(model);
    }

    /// <summary>
    /// Display upgrade plan page
    /// </summary>
    [HttpGet("/account/upgrade")]
    [Authorize]
    public async Task<IActionResult> UpgradePlan()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var profileDto = await _profileService.GetProfileAsync(userId);
        if (profileDto == null)
        {
            return NotFound();
        }

        var model = new AccountLayoutViewModel
        {
            Email = profileDto.Email,
            FirstName = profileDto.FirstName,
            LastName = profileDto.LastName,
            PlanType = profileDto.PlanType.ToString(),
            ApiKeyCount = profileDto.ApiKeyCount,
            MaxApiKeys = profileDto.MaxApiKeys,
            MemberSince = profileDto.CreatedAt
        };

        return View(model);
    }

    /// <summary>
    /// Handle plan upgrade checkout (PayPal integration placeholder)
    /// </summary>
    [HttpPost("/account/checkout")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Checkout(string planType)
    {
        _logger.LogInformation("User requested checkout for plan: {PlanType}", planType);
        
        // TODO: Integrate with PayPal payment processing
        TempData["InfoMessage"] = "PayPal integration is coming soon! You'll be able to upgrade your plan here.";
        
        return RedirectToAction(nameof(UpgradePlan));
    }

    #endregion

    #region Helper Methods

    private static int GetDailyLimitForPlan(PlanType planType) => planType switch
    {
        PlanType.Free => 500,
        PlanType.Premium => 25000,
        PlanType.Pro => 250000,
        _ => 500
    };

    #endregion
}
