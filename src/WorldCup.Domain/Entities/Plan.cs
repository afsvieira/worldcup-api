using WorldCup.Domain.Enums;

namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a subscription plan with its limits and features
/// </summary>
public class Plan
{
    public PlanType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DailyRequestLimit { get; set; }
    public int MinuteRequestLimit { get; set; }
    public int MaxApiKeys { get; set; }
    public bool HasRestAccess { get; set; }
    public bool HasGraphQLAccess { get; set; }
}