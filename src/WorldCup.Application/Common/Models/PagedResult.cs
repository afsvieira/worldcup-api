namespace WorldCup.Application.Common.Models;

/// <summary>
/// Represents a paginated result set
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The items in the current page
    /// </summary>
    public IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// The current page number (1-based)
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total number of items across all pages
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    public PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates an empty paged result
    /// </summary>
    public static PagedResult<T> Empty(int page, int pageSize) =>
        new(Array.Empty<T>(), page, pageSize, 0);
}
