using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WorldCup.API.Models;
using WorldCup.Shared.Extensions;

namespace WorldCup.API.Controllers;

/// <summary>
/// Provides health check endpoints for monitoring API status and dependencies.
/// </summary>
/// <remarks>
/// This controller exposes endpoints to check the overall health of the API
/// and its critical components such as databases, caches, and external services.
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Tags("Health & Monitoring")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<HealthController> _logger;
    private readonly StartupTimeService _startupTimeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthController"/> class.
    /// </summary>
    /// <param name="healthCheckService">The health check service for executing health checks.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="startupTimeService">The startup time tracking service.</param>
    public HealthController(
        HealthCheckService healthCheckService,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<HealthController> logger,
        StartupTimeService startupTimeService)
    {
        _healthCheckService = healthCheckService;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
        _startupTimeService = startupTimeService;
    }

    /// <summary>
    /// Retrieves the overall health status of the API and all registered components.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/health
    /// 
    /// This endpoint performs health checks on all registered components including:
    /// - Environment verification
    /// - Uptime tracking
    /// - Database connectivity (when configured)
    /// - External service dependencies (when configured)
    /// 
    /// The response includes the overall status, API version, environment name,
    /// uptime duration, and detailed status for each component.
    /// </remarks>
    /// <returns>
    /// A health check response containing the overall status and component-level details.
    /// </returns>
    /// <response code="200">
    /// The API is healthy. All critical components are operational.
    /// Returns detailed health information including status, version, environment, uptime, and component statuses.
    /// </response>
    /// <response code="503">
    /// The API is unhealthy or degraded. One or more critical components are not operational.
    /// Returns the same structure as 200 but with unhealthy status indicators.
    /// </response>
    /// <response code="500">
    /// An unexpected error occurred while performing the health check.
    /// Returns error details.
    /// </response>
    [HttpGet(Name = "GetHealthStatus")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Use Debug level for health check start (avoid noise in production)
            _logger.LogDebug("Health check requested");

            var healthCheckResult = await _healthCheckService.CheckHealthAsync();

            // Collect component statuses
            var components = healthCheckResult.Entries.Select(entry => new HealthCheckResponse.ComponentStatus(
                Name: entry.Key,
                Status: entry.Value.Status.ToString(),
                LastChecked: DateTime.UtcNow,
                Details: entry.Value.Description
            )).ToList();

            // Build the response DTO
            var response = new HealthCheckResponse
            {
                Status = healthCheckResult.Status.ToString(),
                ApiVersion = _configuration["App:Version"] ?? "1.0",
                Environment = _environment.EnvironmentName,
                Uptime = (DateTime.UtcNow - _startupTimeService.StartupTime).ToString(@"hh\:mm\:ss\.fff"),
                Components = components
            };

            // Log based on health status
            if (healthCheckResult.Status == HealthStatus.Unhealthy)
            {
                // ERROR level for unhealthy status
                _logger.LogError(
                    "Health check FAILED with status {HealthStatus}. Unhealthy components: {UnhealthyComponents}",
                    healthCheckResult.Status,
                    string.Join(", ", components.Where(c => c.Status != "Healthy").Select(c => c.Name)));
                
                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
            else if (healthCheckResult.Status == HealthStatus.Degraded)
            {
                // WARNING level for degraded status
                _logger.LogWarning(
                    "Health check completed with DEGRADED status. Affected components: {DegradedComponents}",
                    string.Join(", ", components.Where(c => c.Status == "Degraded").Select(c => c.Name)));
            }
            else
            {
                // DEBUG level for successful health checks (reduce noise)
                _logger.LogDebug(
                    "Health check completed successfully. Components checked: {ComponentCount}",
                    components.Count);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            // ERROR level with full exception details
            _logger.LogError(
                ex,
                "Unhandled exception occurred during health check. ExceptionType: {ExceptionType}",
                ex.GetType().Name);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Status = "Error",
                Message = "An error occurred while checking health status"
            });
        }
    }
}
