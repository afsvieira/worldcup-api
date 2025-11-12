using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
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
        
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        
        services.AddSwaggerGen(options =>
        {
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
            
            // Replace {version} in paths with the actual version
            options.OperationFilter<RemoveVersionParameterFilter>();
            options.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
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
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
        }

        return app;
    }
}

/// <summary>
/// Configures Swagger options with API versioning support
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Version = description.ApiVersion.ToString(),
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
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}

/// <summary>
/// Removes the version parameter from Swagger operations
/// </summary>
public class RemoveVersionParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var versionParameter = operation.Parameters.FirstOrDefault(p => 
            p.Name == "version" && p.In == ParameterLocation.Path);

        if (versionParameter != null)
        {
            operation.Parameters.Remove(versionParameter);
        }
    }
}

/// <summary>
/// Replaces {version} in the path with the actual version value
/// </summary>
public class ReplaceVersionWithExactValueInPathFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();

        foreach (var path in swaggerDoc.Paths)
        {
            paths.Add(
                path.Key.Replace("{version}", swaggerDoc.Info.Version, StringComparison.OrdinalIgnoreCase),
                path.Value);
        }

        swaggerDoc.Paths = paths;
    }
}
