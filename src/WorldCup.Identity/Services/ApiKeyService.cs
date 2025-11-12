using System.Security.Cryptography;
using WorldCup.Domain.Entities;

namespace WorldCup.Identity.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IUserService _userService;
    private readonly IPlanService _planService;
    private readonly List<ApiKey> _apiKeys = new();

    public ApiKeyService(IUserService userService, IPlanService planService)
    {
        _userService = userService;
        _planService = planService;
    }

    public async Task<string> GenerateApiKeyAsync(string entraExternalId, string name)
    {
        var user = await _userService.GetUserByExternalIdAsync(entraExternalId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var currentApiKeyCount = _apiKeys.Count(k => k.UserId == user.Id && k.IsActive);
        if (!_planService.CanCreateApiKey(user.PlanType, currentApiKeyCount))
            throw new InvalidOperationException($"API key limit reached for {user.PlanType} plan");

        var apiKey = GenerateSecureApiKey();
        
        var apiKeyEntity = new ApiKey
        {
            Id = Guid.NewGuid(),
            Key = apiKey,
            Name = name,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _apiKeys.Add(apiKeyEntity);
        return apiKey;
    }

    public Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        var exists = _apiKeys.Any(k => k.Key == apiKey && k.IsActive);
        return Task.FromResult(exists);
    }

    public Task<Guid?> GetUserIdByApiKeyAsync(string apiKey)
    {
        var key = _apiKeys.FirstOrDefault(k => k.Key == apiKey && k.IsActive);
        return Task.FromResult(key?.UserId);
    }

    private static string GenerateSecureApiKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}