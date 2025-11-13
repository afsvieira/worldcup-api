namespace WorldCup.Application.DTOs;

/// <summary>
/// Standard error response.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error details.
    /// </summary>
    public ErrorDetail Error { get; set; } = new();
}

/// <summary>
/// Error detail information.
/// </summary>
public class ErrorDetail
{
    /// <summary>
    /// Error code (e.g., "VALIDATION_ERROR", "NOT_FOUND", "UNAUTHORIZED").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Additional error details (optional).
    /// </summary>
    public List<ErrorFieldDetail>? Details { get; set; }
}

/// <summary>
/// Field-specific error detail.
/// </summary>
public class ErrorFieldDetail
{
    /// <summary>
    /// Field name.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Issue description.
    /// </summary>
    public string Issue { get; set; } = string.Empty;
}
