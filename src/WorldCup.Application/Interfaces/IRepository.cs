using System.Linq.Expressions;
using WorldCup.Application.Common.Models;
using WorldCup.Domain.Entities;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Generic repository interface for data access
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with pagination
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match a predicate
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match a predicate with pagination
    /// </summary>
    Task<PagedResult<T>> FindPagedAsync(
        Expression<Func<T, bool>> predicate,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of entities
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of entities that match a predicate
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the predicate
    /// </summary>
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
}
