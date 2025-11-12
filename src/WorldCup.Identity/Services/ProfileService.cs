using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorldCup.Application.DTOs.Common;
using WorldCup.Application.DTOs.Requests.Profile;
using WorldCup.Application.DTOs.Responses;
using WorldCup.Application.Interfaces;
using WorldCup.Identity.Data;
using WorldCup.Identity.Models;
using WorldCup.Domain.Enums;

namespace WorldCup.Identity.Services;

/// <summary>
/// Service for handling user profile operations
/// </summary>
public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<ProfileService> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task<ProfileDto?> GetProfileAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.ApiKeys)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", userId);
            return null;
        }

        var maxApiKeys = GetMaxApiKeysForPlan(user.PlanType);
        var maxMonthlyRequests = GetMaxMonthlyRequestsForPlan(user.PlanType);

        return new ProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PlanType = user.PlanType,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt,
            ApiKeyCount = user.ApiKeys.Count(k => k.IsActive),
            MaxApiKeys = maxApiKeys,
            MonthlyRequests = 0, // TODO: Implement usage tracking
            MaxMonthlyRequests = maxMonthlyRequests
        };
    }

    public async Task<ServiceResult> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Failed("User not found.");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to update profile for user {UserId}", userId);
            return ServiceResult.Failed(result.Errors.Select(e => e.Description));
        }

        _logger.LogInformation("Profile updated for user {Email}", user.Email);
        return ServiceResult.Successful("Profile updated successfully!");
    }

    private static int GetMaxApiKeysForPlan(PlanType planType) => planType switch
    {
        PlanType.Free => 1,
        PlanType.Premium => 3,
        PlanType.Pro => 10,
        _ => 1
    };

    private static int GetMaxMonthlyRequestsForPlan(PlanType planType) => planType switch
    {
        PlanType.Free => 1000,
        PlanType.Premium => 10000,
        PlanType.Pro => 100000,
        _ => 1000
    };
}
