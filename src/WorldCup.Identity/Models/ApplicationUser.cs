using Microsoft.AspNetCore.Identity;
using WorldCup.Domain.Enums;

namespace WorldCup.Identity.Models;

/// <summary>
/// Extended user model with additional properties
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public WorldCup.Domain.Enums.PlanType PlanType { get; set; } = WorldCup.Domain.Enums.PlanType.Free;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public ICollection<UserApiKey> ApiKeys { get; set; } = new List<UserApiKey>();
}