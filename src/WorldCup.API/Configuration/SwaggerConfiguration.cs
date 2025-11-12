using Microsoft.OpenApi.Models;
using System.Reflection;

namespace WorldCup.API.Configuration;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Adds and configures Swagger/OpenAPI
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "WorldCup API",
                Description = "A comprehensive API for managing and accessing FIFA World Cup data, including matches, teams, players, and statistics.",
                Contact = new OpenApiContact
                {
                    Name = "WorldCup API Team",
                    Email = "afsvieira@afsvieira.com",
                    Url = new Uri("https://github.com/afsvieira/worldcup-api")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Filter out UI controllers from Swagger documentation
            // Only show API controllers (those in api/ routes)
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                // Only include endpoints that start with "api/"
                var isApiRoute = apiDesc.RelativePath?.StartsWith("api/", StringComparison.OrdinalIgnoreCase) ?? false;
                return isApiRoute;
            });

            // Include XML comments for better documentation
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Enable annotations for additional metadata
            options.EnableAnnotations();

            // Order actions by relative path
            options.OrderActionsBy(apiDesc => $"{apiDesc.RelativePath}");
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger UI middleware
    /// </summary>
    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
