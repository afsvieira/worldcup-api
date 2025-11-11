# WorldCup API - Logging & Observability

## Overview

This API implements **structured logging** using **Serilog** with comprehensive observability features including correlation IDs, request tracking, and JSON-formatted logs.

## Features

### 🎯 Structured Logging
- **JSON Format**: Logs are formatted in JSON for easy parsing and querying
- **Multiple Sinks**: Console (for development) and File (for production)
- **Log Levels**: Configurable per namespace

### 🔍 Request Tracking
- **CorrelationId**: Tracks requests across distributed systems
- **RequestId**: ASP.NET Core trace identifier
- **TraceId/SpanId**: OpenTelemetry-compatible distributed tracing
- **ClientIp**: Tracks the originating IP address
- **UserAgent**: Captures client information

### 📊 Enrichment Properties

Every log entry automatically includes:

| Property | Description | Example |
|----------|-------------|---------|
| `CorrelationId` | Unique ID for tracking requests across services | `550e8400-e29b-41d4-a716-446655440000` |
| `RequestId` | ASP.NET Core request identifier | `0HMVD8QKR9S7L:00000001` |
| `TraceId` | W3C Trace Context trace identifier | `4bf92f3577b34da6a3ce929d0e0e4736` |
| `SpanId` | W3C Trace Context span identifier | `00f067aa0ba902b7` |
| `RequestPath` | HTTP request path | `/api/v1/health` |
| `RequestMethod` | HTTP method | `GET` |
| `ClientIp` | Client IP address (respects X-Forwarded-For) | `192.168.1.100` |
| `UserAgent` | Client user agent | `Mozilla/5.0...` |
| `Application` | Application name | `WorldCup.API` |
| `Environment` | Deployment environment | `Development` |
| `ApiVersion` | API version | `1.0.0` |
| `MachineName` | Host machine name | `WEB-SERVER-01` |
| `ThreadId` | Thread identifier | `42` |
| `ProcessId` | Process identifier | `12345` |

## Configuration

### appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/worldcup-api-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  }
}
```

## Usage Examples

### Basic Logging

```csharp
_logger.LogInformation("User logged in successfully");
```

### Structured Logging with Properties

```csharp
_logger.LogInformation(
    "Health check completed with status {HealthStatus}. Components checked: {ComponentCount}",
    healthCheckResult.Status,
    components.Count);
```

### Error Logging with Exception

```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error occurred while processing request for {UserId}", userId);
}
```

## Correlation ID Flow

### Client sends request with correlation ID:
```http
GET /api/v1/health HTTP/1.1
X-Correlation-Id: 550e8400-e29b-41d4-a716-446655440000
```

### API responds with the same correlation ID:
```http
HTTP/1.1 200 OK
X-Correlation-Id: 550e8400-e29b-41d4-a716-446655440000
```

### All logs include the correlation ID:
```json
{
  "Timestamp": "2025-11-11T10:30:00.123Z",
  "Level": "Information",
  "MessageTemplate": "Health check requested",
  "CorrelationId": "550e8400-e29b-41d4-a716-446655440000",
  "RequestId": "0HMVD8QKR9S7L:00000001",
  "RequestPath": "/api/v1/health",
  "RequestMethod": "GET"
}
```

## Log Files

- **Location**: `logs/` directory
- **Format**: `worldcup-api-YYYYMMDD.log`
- **Rotation**: Daily
- **Retention**: 30 days (configurable)
- **Format**: Compact JSON (production) or text (development)

## Best Practices

### ✅ DO
- Use structured logging with named parameters
- Include relevant context (IDs, counts, states)
- Log at appropriate levels (Debug, Info, Warning, Error, Fatal)
- Use correlation IDs for distributed tracing

### ❌ DON'T
- Log sensitive data (passwords, tokens, PII)
- Use string interpolation in log messages
- Log excessively in tight loops
- Mix log levels inappropriately

## Integration with Monitoring Tools

These logs are compatible with:
- **Elasticsearch/Kibana** - via JSON format
- **Seq** - native Serilog support
- **Application Insights** - via Serilog sink
- **Datadog** - via JSON file ingestion
- **Splunk** - via JSON format

## Performance Considerations

- Logs are written asynchronously (non-blocking)
- Console logging is minimized in production
- File rolling prevents unbounded disk usage
- Log levels filter out unnecessary entries

## Example Log Output

### Console (Development)
```
[10:30:00 INF] [550e8400-e29b-41d4-a716-446655440000] Health check requested {"RequestPath": "/api/v1/health", "RequestMethod": "GET"}
[10:30:00 INF] [550e8400-e29b-41d4-a716-446655440000] Health check completed with status Healthy. Components checked: 2 {"HealthStatus": "Healthy", "ComponentCount": 2}
```

### File (Compact JSON)
```json
{
  "@t": "2025-11-11T10:30:00.123Z",
  "@l": "Information",
  "@mt": "Health check completed with status {HealthStatus}. Components checked: {ComponentCount}",
  "HealthStatus": "Healthy",
  "ComponentCount": 2,
  "CorrelationId": "550e8400-e29b-41d4-a716-446655440000",
  "RequestId": "0HMVD8QKR9S7L:00000001",
  "TraceId": "4bf92f3577b34da6a3ce929d0e0e4736",
  "SpanId": "00f067aa0ba902b7",
  "RequestPath": "/api/v1/health",
  "RequestMethod": "GET",
  "ClientIp": "192.168.1.100",
  "Application": "WorldCup.API",
  "Environment": "Development",
  "ApiVersion": "1.0.0"
}
```

## Troubleshooting

### No logs appearing
- Check `Serilog:MinimumLevel` configuration
- Verify write permissions to `logs/` directory
- Check console output for Serilog configuration errors

### Missing properties
- Ensure `LogEnrichmentMiddleware` is registered
- Verify Serilog enrichers are configured
- Check `UseSerilogRequestLogging()` is called

### Performance issues
- Reduce log level in production
- Disable console logging in production
- Increase file buffer size
- Consider async file writes
