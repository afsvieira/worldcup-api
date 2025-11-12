using System.Security.Claims;
using WorldCup.Domain.Entities;
using WorldCup.Identity.Services;

namespace WorldCup.Identity.GraphQL;

/// <summary>
/// GraphQL mutations for user registration and API key management
/// </summary>
public class Mutations
{
    public async Task<User> RegisterUser(
        ClaimsPrincipal claimsPrincipal,
        [Service] IUserService userService)
    {
        var externalId = claimsPrincipal.FindFirst("sub")?.Value 
                        ?? claimsPrincipal.FindFirst("oid")?.Value
                        ?? throw new GraphQLException("User ID not found in token");
        
        var email = claimsPrincipal.FindFirst("emails")?.Value 
                   ?? claimsPrincipal.FindFirst("email")?.Value
                   ?? throw new GraphQLException("Email not found in token");
        
        var name = claimsPrincipal.FindFirst("name")?.Value ?? email;

        return await userService.RegisterUserAsync(externalId, email, name);
    }

    public async Task<string> GenerateApiKey(
        string name,
        ClaimsPrincipal claimsPrincipal,
        [Service] IApiKeyService apiKeyService)
    {
        var externalId = claimsPrincipal.FindFirst("sub")?.Value 
                        ?? claimsPrincipal.FindFirst("oid")?.Value
                        ?? throw new GraphQLException("User ID not found in token");

        return await apiKeyService.GenerateApiKeyAsync(externalId, name);
    }
}