using Microsoft.Extensions.Logging;

namespace WorldCup.Shared.Extensions;

public class StartupTimeService
{
    private readonly ILogger<StartupTimeService> _logger;
    private DateTime _startupTime = DateTime.MinValue; // Sentinel value (not initialized)
    public DateTime StartupTime => _startupTime;

    public StartupTimeService(ILogger<StartupTimeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Initializes the startup time (only once).
    /// </summary>
    /// <param name="startupTime">Actual startup time.</param>
    public void Initialize(DateTime startupTime)
    {
        if (_startupTime != DateTime.MinValue)
        {
            _logger.LogWarning("StartupTimeService has already been initialized with {ExistingTime}. New value {NewTime} will be ignored.", _startupTime, startupTime);
            return;
        }
        _startupTime = startupTime;
        _logger.LogInformation("StartupTimeService successfully initialized. Time: {StartupTime}", _startupTime);
    }
}