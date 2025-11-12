using System.Security.Claims;
using WorldCup.Domain.Entities;
using WorldCup.Domain.Enums;
using WorldCup.Identity.Services;

namespace WorldCup.Identity.GraphQL;

/// <summary>
/// GraphQL queries for user and plan information
/// </summary>
public class Queries
{
    public async Task<User?> Me(
        ClaimsPrincipal claimsPrincipal,
        [Service] IUserService userService)
    {
        var azureId = claimsPrincipal.FindFirst("sub")?.Value 
                     ?? claimsPrincipal.FindFirst("oid")?.Value;

        if (string.IsNullOrEmpty(azureId))
            return null;

        return await userService.GetUserByAzureIdAsync(azureId);
    }

    public IEnumerable<Plan> Plans([Service] IPlanService planService)
    {
        return new[]
        {
            planService.GetPlan(PlanType.Free),
            planService.GetPlan(PlanType.Premium),
            planService.GetPlan(PlanType.Pro)
        };
    }
}