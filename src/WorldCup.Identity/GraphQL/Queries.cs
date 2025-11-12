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
        var externalId = claimsPrincipal.FindFirst("sub")?.Value 
                        ?? claimsPrincipal.FindFirst("oid")?.Value;

        if (string.IsNullOrEmpty(externalId))
            return null;

        return await userService.GetUserByExternalIdAsync(externalId);
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