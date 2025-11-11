using System.ComponentModel.DataAnnotations;

namespace WorldCup.API.Models;

/// <summary>
/// Represents the response structure for the health check endpoint.
/// </summary>
/// <example>
/// {
///   "status": "Healthy",
///   "apiVersion": "1.0",
///   "environment": "Development",
///   "uptime": "02:15:30.123",
///   "components": [
///     {
///       "name": "Environment",
///       "status": "Healthy",
///       "lastChecked": "2025-11-11T10:30:00Z",
///       "details": "Environment: Development"
///     },
///     {
///       "name": "Uptime",
///       "status": "Healthy",
///       "lastChecked": "2025-11-11T10:30:00Z",
///       "details": "Uptime will be calculated at runtime"
///     }
///   ]
/// }
/// </example>
public class HealthCheckResponse
{
    /// <summary>
    /// Overall status of the API.
    /// </summary>
    /// <example>Healthy</example>
    [Required]
    public string Status { get; set; } = "Healthy";

    /// <summary>
    /// Version of the API.
    /// </summary>
    /// <example>1.0</example>
    [Required]
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// Current deployment environment.
    /// </summary>
    /// <example>Development</example>
    [Required]
    public string Environment { get; set; } = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

    /// <summary>
    /// Duration the API has been running since startup (formatted as HH:MM:SS.FFF).
    /// </summary>
    /// <example>02:15:30.123</example>
    [Required]
    public string Uptime { get; set; } = TimeSpan.Zero.ToString();

    /// <summary>
    /// Detailed status of individual components and dependencies.
    /// </summary>
    [Required]
    public List<ComponentStatus> Components { get; set; } = new();

    /// <summary>
    /// Represents the health status of an individual component or dependency.
    /// </summary>
    /// <param name="Name">The name of the component (e.g., "Database", "Cache").</param>
    /// <param name="Status">The health status of the component (Healthy, Degraded, Unhealthy).</param>
    /// <param name="LastChecked">The timestamp when this component was last checked.</param>
    /// <param name="Details">Additional details or diagnostic information about the component status.</param>
    public record ComponentStatus(
        string Name,
        string Status,
        DateTime LastChecked,
        string? Details
    );
}