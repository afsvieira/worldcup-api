namespace WorldCup.Application.Common.Models;

/// <summary>
/// Base class for pagination parameters
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (1-based). Default: 1
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Default: 10, Max: 100
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Calculate the number of items to skip
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Get the number of items to take
    /// </summary>
    public int Take => PageSize;
}
