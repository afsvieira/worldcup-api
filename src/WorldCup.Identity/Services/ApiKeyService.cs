using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorldCup.Application.DTOs.Common;
using WorldCup.Application.DTOs.Responses;
using WorldCup.Application.Interfaces;
using WorldCup.Identity.Data;
using WorldCup.Identity.Models;
using WorldCup.Shared.Services;
using WorldCup.Domain.Enums;

namespace WorldCup.Identity.Services;

/// <summary>
/// Service for handling API key operations
/// </summary>
public class ApiKeyService : IApiKeyService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(
        ApplicationDbContext context,
        ILogger<ApiKeyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ApiKeyDto>> GetApiKeysAsync(string userId)
    {
        var apiKeys = await _context.UserApiKeys
            .Where(k => k.UserId == userId)
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync();

        return apiKeys.Select(k => new ApiKeyDto
        {
            Id = k.Id,
            Name = k.Name,
            KeyPreview = ApiKeyGenerator.MaskApiKey(k.KeyHash),
            IsActive = k.IsActive,
            CreatedAt = k.CreatedAt
        });
    }

    public async Task<ApiKeyCreateResult> CreateApiKeyAsync(string userId, string name)
    {
        // Check user exists and get their plan
        var user = await _context.Users
            .Include(u => u.ApiKeys)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return ApiKeyCreateResult.Failed("User not found.");
        }

        // Check API key limit
        var maxApiKeys = GetMaxApiKeysForPlan(user.PlanType);
        var activeKeyCount = user.ApiKeys.Count(k => k.IsActive);

        if (activeKeyCount >= maxApiKeys)
        {
            return ApiKeyCreateResult.Failed($"You have reached the maximum number of API keys ({maxApiKeys}) for your plan.");
        }

        // Generate new API key
        var plainKey = ApiKeyGenerator.GenerateApiKey();
        var keyHash = ApiKeyGenerator.HashApiKey(plainKey);

        var apiKey = new UserApiKey
        {
            UserId = userId,
            Name = name,
            KeyHash = keyHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();

        _logger.LogInformation("API key created for user {UserId}", userId);

        var dto = new ApiKeyDto
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            KeyPreview = ApiKeyGenerator.MaskApiKey(plainKey),
            IsActive = apiKey.IsActive,
            CreatedAt = apiKey.CreatedAt
        };

        return ApiKeyCreateResult.Successful(plainKey, dto);
    }

    public async Task<ServiceResult> RevokeApiKeyAsync(string userId, Guid apiKeyId)
    {
        var apiKey = await _context.UserApiKeys
            .FirstOrDefaultAsync(k => k.Id == apiKeyId && k.UserId == userId);

        if (apiKey == null)
        {
            return ServiceResult.Failed("API key not found.");
        }

        apiKey.IsActive = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("API key {ApiKeyId} revoked for user {UserId}", apiKeyId, userId);
        return ServiceResult.Successful("API key revoked successfully.");
    }

    public async Task<ServiceResult> DeleteApiKeyAsync(string userId, Guid apiKeyId)
    {
        var apiKey = await _context.UserApiKeys
            .FirstOrDefaultAsync(k => k.Id == apiKeyId && k.UserId == userId);

        if (apiKey == null)
        {
            return ServiceResult.Failed("API key not found.");
        }

        _context.UserApiKeys.Remove(apiKey);
        await _context.SaveChangesAsync();

        _logger.LogInformation("API key {ApiKeyId} deleted for user {UserId}", apiKeyId, userId);
        return ServiceResult.Successful("API key deleted successfully.");
    }

    public async Task<int> GetApiKeyCountAsync(string userId)
    {
        return await _context.UserApiKeys
            .Where(k => k.UserId == userId && k.IsActive)
            .CountAsync();
    }

    private static int GetMaxApiKeysForPlan(PlanType planType) => planType switch
    {
        PlanType.Free => 1,
        PlanType.Premium => 3,
        PlanType.Pro => 10,
        _ => 1
    };
}
