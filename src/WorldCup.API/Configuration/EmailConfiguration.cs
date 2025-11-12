using WorldCup.Shared.Services;

namespace WorldCup.API.Configuration;

/// <summary>
/// Configuration for email services
/// </summary>
public static class EmailConfiguration
{
    /// <summary>
    /// Adds email services to the DI container
    /// </summary>
    public static IServiceCollection AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Register email rate limiter as singleton (2 minutes between emails)
        services.AddSingleton(new EmailRateLimiter(TimeSpan.FromMinutes(2)));
        
        // Check which email provider to use
        var emailProvider = configuration["Email:Provider"] ?? "Azure"; // Default to Azure
        
        if (emailProvider.Equals("Azure", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IEmailSender, AzureEmailSender>();
        }
        else if (emailProvider.Equals("Smtp", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IEmailSender, SmtpEmailSender>();
        }
        else
        {
            // Default to Azure if invalid provider specified
            services.AddScoped<IEmailSender, AzureEmailSender>();
        }
        
        return services;
    }
}
