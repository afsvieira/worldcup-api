namespace WorldCup.Application.DTOs.Responses;

/// <summary>
/// API key data transfer object
/// Contains masked key information for display (never shows full key after creation)
/// </summary>
public class ApiKeyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyPreview { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
