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
        }).AddMvc();

        return services;
    }
}
