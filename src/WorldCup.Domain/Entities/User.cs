using WorldCup.Domain.Enums;

namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a registered user with authentication and subscription information
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string EntraExternalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PlanType PlanType { get; set; } = PlanType.Free;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
}