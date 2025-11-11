# Log Levels Strategy - WorldCup API

## Overview

This document describes the logging strategy for different environments and scenarios to ensure meaningful logs without noise.

## Log Level Guidelines

### 🔴 **ERROR** - Critical issues requiring immediate attention
- **When to use:**
  - Unhandled exceptions
  - HTTP 5xx responses (server errors)
  - Failed health checks (Unhealthy status)
  - Database connection failures
  - External service failures
  - Security violations

- **What to include:**
  - Full exception with stack trace
  - Request context (CorrelationId, RequestId)
  - Structured data for easy querying

**Example:**
```csharp
_logger.LogError(
    ex,
    "Unhandled exception occurred during health check. ExceptionType: {ExceptionType}",
    ex.GetType().Name);
```

### 🟡 **WARNING** - Potential issues or degraded functionality
- **When to use:**
  - HTTP 4xx responses (except 401, 404 which are normal)
  - Degraded health check status
  - Performance issues (slow queries)
  - Fallback mechanisms activated
  - Configuration issues (non-critical)

**Example:**
```csharp
_logger.LogWarning(
    "Health check completed with DEGRADED status. Affected components: {DegradedComponents}",
    string.Join(", ", degradedComponents));
```

### 🔵 **INFORMATION** - Significant business events
- **When to use:**
  - Application startup/shutdown
  - HTTP 2xx, 3xx responses
  - Important business operations completed
  - Configuration loaded
  - Authentication events

**Example:**
```csharp
_logger.LogInformation(
    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms",
    method, path, statusCode, elapsed);
```

### ⚪ **DEBUG** - Detailed diagnostic information
- **When to use:**
  - Health check requests (reduce noise)
  - Detailed flow information
  - Variable values during troubleshooting
  - **Only enabled in Development**

**Example:**
```csharp
_logger.LogDebug("Health check requested");
```

### ⚫ **TRACE** - Very detailed diagnostic information
- **When to use:**
  - Extremely detailed flow
  - Every step of complex operations
  - **Typically never used in production**

## Environment-Specific Configuration

### 📦 **Development**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "System": "Information",
        "WorldCup": "Debug"
      }
    },
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithThreadId",
      "WithProcessId",
      "WithEnvironmentName"
    ]
  }
}
```

**Characteristics:**
- ✅ Verbose logging (Debug level)
- ✅ Detailed enrichment (ThreadId, ProcessId, etc.)
- ✅ Pretty-printed console output
- ✅ Stack traces included
- ✅ Short retention (7 days)

### 🏭 **Production**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Warning",
        "System": "Error",
        "WorldCup": "Information"
      }
    },
    "Enrich": [
      "FromLogContext",
      "WithEnvironmentName"
    ]
  }
}
```

**Characteristics:**
- ✅ Minimal logging (Warning+ level)
- ✅ Essential enrichment only
- ✅ Compact JSON format
- ✅ No sensitive data
- ✅ Long retention (90 days)

## HTTP Status Code Mapping

| Status Code | Log Level | Example |
|-------------|-----------|---------|
| 200-299 (Success) | Information | `[INF] HTTP GET /api/v1/matches responded 200` |
| 300-399 (Redirect) | Information | `[INF] HTTP GET /api/v1/teams responded 301` |
| 401 (Unauthorized) | Information | Common, not an error |
| 404 (Not Found) | Information | Common, not an error |
| 400-499 (Client Error) | Warning | `[WRN] HTTP POST /api/v1/teams responded 400` |
| 500-599 (Server Error) | Error | `[ERR] HTTP GET /api/v1/matches responded 500` |
| Unhandled Exception | Error | Full exception logged |

## Health Check Logging

| Status | Log Level | Behavior |
|--------|-----------|----------|
| Healthy | Debug | Logged at Debug (reduce noise) |
| Degraded | Warning | Logged with affected components |
| Unhealthy | Error | Logged with full details |

**Production behavior:**
- Health checks logged at Debug level (filtered out)
- Only failures (Degraded/Unhealthy) appear in logs

## Structured Logging Examples

### ✅ Good - Structured with context
```csharp
_logger.LogError(
    ex,
    "Failed to process order {OrderId} for customer {CustomerId}. Reason: {FailureReason}",
    orderId,
    customerId,
    ex.Message);
```

### ❌ Bad - String interpolation
```csharp
_logger.LogError($"Failed to process order {orderId} for customer {customerId}");
```

### ✅ Good - Conditional logging
```csharp
if (_logger.IsEnabled(LogLevel.Debug))
{
    _logger.LogDebug("Processing {Count} items: {@Items}", items.Count, items);
}
```

## Request Logging

All HTTP requests are automatically logged with:

**Always included:**
- RequestMethod (GET, POST, etc.)
- RequestPath (/api/v1/health)
- StatusCode (200, 404, 500)
- Elapsed time (milliseconds)
- CorrelationId
- RequestId

**Development only:**
- ClientIp
- UserAgent
- TraceId/SpanId
- ThreadId/ProcessId

**Example output (Production):**
```
[10:30:15 INF] HTTP GET /api/v1/matches responded 200 in 45.2300 ms
[10:30:16 WRN] HTTP POST /api/v1/teams responded 400 in 12.1100 ms
[10:30:17 ERR] HTTP GET /api/v1/players responded 500 in 1024.5600 ms
```

## Exception Handling Strategy

### Global Exception Handler
- Catches all unhandled exceptions
- Logs with ERROR level + full stack trace
- Returns sanitized error response
- Includes details only in Development

### Controller-Level
- Catch specific exceptions for business logic
- Log with appropriate level (Warning/Error)
- Return meaningful error responses

## Performance Considerations

### ✅ Production Optimizations
1. **Minimal enrichment** - Only essential properties
2. **Higher log levels** - Filter out Debug/Information
3. **Async file writes** - Non-blocking I/O
4. **Compact JSON** - Smaller log files
5. **Reduced console logging** - File-only in production

### 📊 Metrics
- **Development**: ~500-1000 logs/minute (verbose)
- **Production**: ~50-100 logs/minute (filtered)
- **File size**: ~10-50 MB/day (production)

## Monitoring & Alerts

### Critical Alerts (ERROR level)
- Unhandled exceptions
- Health check failures
- HTTP 5xx spikes
- External service failures

### Warning Alerts
- Degraded health checks
- High 4xx rate
- Slow response times

## Best Practices

### ✅ DO
- Use structured logging with named parameters
- Include CorrelationId in all logs
- Log exceptions at ERROR level
- Use appropriate log levels
- Filter health checks to Debug level
- Include context (IDs, counts, states)

### ❌ DON'T
- Log passwords, tokens, or PII
- Use string interpolation
- Log excessively in tight loops
- Mix log levels inappropriately
- Include sensitive data in logs
- Log at Information for every health check

## Querying Logs

### Find all errors for a request:
```bash
grep "CorrelationId.*550e8400" worldcup-api-20251111.log | grep ERR
```

### Find failed health checks:
```bash
grep "Health check FAILED" worldcup-api-*.log
```

### Find slow requests (>1000ms):
```bash
grep "responded.*in [1-9][0-9][0-9][0-9]" worldcup-api-*.log
```

## Retention Policy

| Environment | Retention | Rationale |
|-------------|-----------|-----------|
| Development | 7 days | Fast iteration, disk space |
| Staging | 30 days | Testing validation |
| Production | 90 days | Compliance, debugging |
