using WorldCup.Domain.Enums;

namespace WorldCup.Application.DTOs.Responses;

/// <summary>
/// User profile data transfer object
/// Contains all user profile information and plan details
/// </summary>
public class ProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public PlanType PlanType { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ApiKeyCount { get; set; }
    public int MaxApiKeys { get; set; }
    public int MonthlyRequests { get; set; }
    public int MaxMonthlyRequests { get; set; }
}
