using System.Security.Cryptography;
using System.Text;

namespace WorldCup.Shared.Services;

/// <summary>
/// Service for generating and hashing API keys
/// </summary>
public static class ApiKeyGenerator
{
    private const string Prefix = "wc_"; // WorldCup prefix
    private const int KeyLength = 32; // 32 characters after prefix
    
    /// <summary>
    /// Generate a new API key
    /// Format: wc_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (prefix + 32 random chars)
    /// </summary>
    public static string GenerateApiKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder(Prefix);
        
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[KeyLength];
        rng.GetBytes(buffer);
        
        for (int i = 0; i < KeyLength; i++)
        {
            stringBuilder.Append(chars[buffer[i] % chars.Length]);
        }
        
        return stringBuilder.ToString();
    }
    
    /// <summary>
    /// Hash an API key for secure storage
    /// Uses SHA256 for hashing
    /// </summary>
    public static string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(apiKey);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    
    /// <summary>
    /// Verify if a given API key matches the stored hash
    /// </summary>
    public static bool VerifyApiKey(string apiKey, string storedHash)
    {
        var computedHash = HashApiKey(apiKey);
        return computedHash == storedHash;
    }
    
    /// <summary>
    /// Get masked version of API key for display
    /// Shows only prefix and last 4 characters: wc_****************************abcd
    /// </summary>
    public static string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 8)
        {
            return "****";
        }
        
        // If it has prefix, show prefix + masked middle + last 4
        if (apiKey.StartsWith(Prefix))
        {
            var last4 = apiKey[^4..];
            var maskedLength = apiKey.Length - Prefix.Length - 4;
            return $"{Prefix}{"*".PadLeft(maskedLength, '*')}{last4}";
        }
        
        // Otherwise just show last 4
        return $"****{apiKey[^4..]}";
    }
    
    /// <summary>
    /// Get last N characters of API key for identification
    /// </summary>
    public static string GetKeyPreview(string apiKey, int lastChars = 4)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < lastChars)
        {
            return apiKey;
        }
        
        return $"...{apiKey[^lastChars..]}";
    }
}
