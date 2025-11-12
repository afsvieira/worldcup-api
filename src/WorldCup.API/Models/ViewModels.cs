using System.ComponentModel.DataAnnotations;

namespace WorldCup.API.Models;

/// <summary>
/// View model for user registration
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// View model for user login
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}

/// <summary>
/// View model for user profile
/// </summary>
public class ProfileViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public DateTime MemberSince { get; set; }
    public bool EmailConfirmed { get; set; }
    public WorldCup.Domain.Enums.PlanType PlanType { get; set; }
    public int RequestsToday { get; set; }
    public int RequestsThisWeek { get; set; }
    public int RequestsThisMonth { get; set; }
    public int DailyLimit { get; set; }
    public int UsagePercentage => DailyLimit > 0 ? (int)((double)RequestsToday / DailyLimit * 100) : 0;
    public int ApiKeyCount { get; set; }
    public int MaxApiKeys { get; set; }
}

/// <summary>
/// View model for changing password
/// </summary>
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your new password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// View model for API keys management
/// </summary>
public class ApiKeysViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public WorldCup.Domain.Enums.PlanType PlanType { get; set; }
    public int MaxApiKeys { get; set; }
    public int ApiKeyCount => ApiKeys.Count;
    public int MaxMonthlyRequests { get; set; }
    public DateTime MemberSince { get; set; }
    public List<ApiKeyItemViewModel> ApiKeys { get; set; } = new();
    
    /// <summary>
    /// Newly created API key - shown only once
    /// </summary>
    public string? NewlyCreatedApiKey { get; set; }
    public string? NewlyCreatedKeyName { get; set; }
}

/// <summary>
/// View model for individual API key
/// </summary>
public class ApiKeyItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Masked version of the key (e.g., wc_****************************abcd)
    /// </summary>
    public string KeyPreview { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}

/// <summary>
/// Base view model for account layout (shared sidebar)
/// </summary>
public class AccountLayoutViewModel
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string PlanType { get; set; } = "Free";
    public int ApiKeyCount { get; set; }
    public int MaxApiKeys { get; set; }
    public DateTime MemberSince { get; set; }
}
