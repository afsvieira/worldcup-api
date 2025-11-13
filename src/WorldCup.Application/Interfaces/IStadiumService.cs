using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Stadium service interface.
/// </summary>
public interface IStadiumService
{
    /// <summary>
    /// Gets stadiums with optional filtering and pagination.
    /// </summary>
    /// <param name="city">Filter by city (optional).</param>
    /// <param name="country">Filter by country (optional).</param>
    /// <param name="pagination">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged stadium results.</returns>
    Task<PagedResult<StadiumDto>> GetAllAsync(
        string? city,
        string? country,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a stadium by ID.
    /// </summary>
    /// <param name="id">Stadium ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stadium or null if not found.</returns>
    Task<StadiumDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
