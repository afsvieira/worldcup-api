namespace WorldCup.Identity.Models;

/// <summary>
/// API key associated with a user
/// </summary>
public class UserApiKey
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ApplicationUser User { get; set; } = null!;
}