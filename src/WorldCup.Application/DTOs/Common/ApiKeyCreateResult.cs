using WorldCup.Application.DTOs.Responses;

namespace WorldCup.Application.DTOs.Common;

/// <summary>
/// Result for API key creation operations
/// Includes plain text key (shown only once) and key metadata
/// </summary>
public class ApiKeyCreateResult : ServiceResult
{
    public string? PlainKey { get; set; }
    public ApiKeyDto? ApiKey { get; set; }

    public static ApiKeyCreateResult Successful(string plainKey, ApiKeyDto apiKey)
        => new() { Success = true, PlainKey = plainKey, ApiKey = apiKey };

    public static new ApiKeyCreateResult Failed(string message)
        => new() { Success = false, Message = message };
}
