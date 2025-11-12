using WorldCup.Domain.Entities;
using WorldCup.Domain.Enums;

namespace WorldCup.Identity.Services;

public class PlanService : IPlanService
{
    private readonly Dictionary<PlanType, Plan> _plans = new()
    {
        [PlanType.Free] = new Plan
        {
            Type = PlanType.Free,
            Name = "Free",
            Price = 0,
            DailyRequestLimit = 500,
            MinuteRequestLimit = 10,
            MaxApiKeys = 1,
            HasRestAccess = true,
            HasGraphQLAccess = false
        },
        [PlanType.Premium] = new Plan
        {
            Type = PlanType.Premium,
            Name = "Premium",
            Price = 9.99m,
            DailyRequestLimit = 25000,
            MinuteRequestLimit = 100,
            MaxApiKeys = 3,
            HasRestAccess = true,
            HasGraphQLAccess = false
        },
        [PlanType.Pro] = new Plan
        {
            Type = PlanType.Pro,
            Name = "Pro",
            Price = 49.99m,
            DailyRequestLimit = 250000,
            MinuteRequestLimit = 1000,
            MaxApiKeys = 10,
            HasRestAccess = true,
            HasGraphQLAccess = true
        }
    };

    public Plan GetPlan(PlanType planType) => _plans[planType];

    public bool CanCreateApiKey(PlanType planType, int currentApiKeyCount)
    {
        var plan = GetPlan(planType);
        return currentApiKeyCount < plan.MaxApiKeys;
    }

    public bool CanMakeRequest(PlanType planType, int dailyRequestCount, int minuteRequestCount)
    {
        var plan = GetPlan(planType);
        return dailyRequestCount < plan.DailyRequestLimit && minuteRequestCount < plan.MinuteRequestLimit;
    }
}