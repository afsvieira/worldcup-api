using WorldCup.Domain.Entities;
using WorldCup.Domain.Enums;

namespace WorldCup.Identity.Services;

/// <summary>
/// Service for managing subscription plans and their limits
/// </summary>
public interface IPlanService
{
    /// <summary>
    /// Gets plan configuration by type
    /// </summary>
    Plan GetPlan(PlanType planType);
    
    /// <summary>
    /// Checks if user can create another API key based on plan limits
    /// </summary>
    bool CanCreateApiKey(PlanType planType, int currentApiKeyCount);
    
    /// <summary>
    /// Checks if user can make requests based on plan limits
    /// </summary>
    bool CanMakeRequest(PlanType planType, int dailyRequestCount, int minuteRequestCount);
}