namespace WorldCup.Application.DTOs;

/// <summary>
/// Standard API response wrapper.
/// </summary>
/// <typeparam name="T">Response data type.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Response data.
    /// </summary>
    public T Data { get; set; } = default!;

    /// <summary>
    /// Response metadata.
    /// </summary>
    public ApiResponseMeta? Meta { get; set; }

    /// <summary>
    /// Pagination links (only for paginated responses).
    /// </summary>
    public ApiResponseLinks? Links { get; set; }
}

/// <summary>
/// Response metadata.
/// </summary>
public class ApiResponseMeta
{
    /// <summary>
    /// Response timestamp (ISO 8601).
    /// </summary>
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");

    /// <summary>
    /// Current page number (paginated responses only).
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    /// Page size (paginated responses only).
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Total item count (paginated responses only).
    /// </summary>
    public int? TotalCount { get; set; }

    /// <summary>
    /// Total page count (paginated responses only).
    /// </summary>
    public int? TotalPages { get; set; }
}

/// <summary>
/// Pagination links.
/// </summary>
public class ApiResponseLinks
{
    /// <summary>
    /// Current page URL.
    /// </summary>
    public string Self { get; set; } = string.Empty;

    /// <summary>
    /// Next page URL (null if last page).
    /// </summary>
    public string? Next { get; set; }

    /// <summary>
    /// Previous page URL (null if first page).
    /// </summary>
    public string? Prev { get; set; }

    /// <summary>
    /// First page URL.
    /// </summary>
    public string First { get; set; } = string.Empty;

    /// <summary>
    /// Last page URL.
    /// </summary>
    public string Last { get; set; } = string.Empty;
}
