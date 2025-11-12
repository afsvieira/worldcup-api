using Asp.Versioning;

namespace WorldCup.API.Configuration;

/// <summary>
/// Extension methods for configuring API versioning
/// </summary>
public static class VersioningConfiguration
{
    /// <summary>
    /// Adds and configures API versioning
    /// </summary>
    public static IServiceCollection AddVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader()
            );
            options.ReportApiVersions = true;
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            // Add the versioned API explorer, which adds IApiVersionDescriptionProvider service
            // Format the version as 'v'major[.minor][-status]
            options.GroupNameFormat = "'v'VVV";
            
            // Substitute the version route parameter in swagger doc
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}
