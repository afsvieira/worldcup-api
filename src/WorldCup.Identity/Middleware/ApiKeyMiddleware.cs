using System.Security.Claims;
using WorldCup.Identity.Services;

namespace WorldCup.Identity.Middleware;

/// <summary>
/// Middleware for validating API keys in REST requests
/// </summary>
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        if (context.Request.Path.StartsWithSegments("/graphql"))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
        {
            var apiKey = authHeader["Bearer ".Length..];
            
            if (await apiKeyService.ValidateApiKeyAsync(apiKey))
            {
                var userId = await apiKeyService.GetUserIdByApiKeyAsync(apiKey);
                if (userId.HasValue)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                        new Claim("api_key", apiKey)
                    };
                    
                    var identity = new ClaimsIdentity(claims, "ApiKey");
                    context.User = new ClaimsPrincipal(identity);
                }
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key required");
            return;
        }

        await _next(context);
    }
}