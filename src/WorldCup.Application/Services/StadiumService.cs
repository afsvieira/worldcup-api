using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;
using WorldCup.Application.Interfaces;
using WorldCup.Domain.Entities;

namespace WorldCup.Application.Services;

/// <summary>
/// Stadium service implementation with Redis caching.
/// </summary>
public class StadiumService : IStadiumService
{
    private readonly IRepository<Stadium> _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<StadiumService> _logger;
    private const string CacheKeyPrefix = "stadiums:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public StadiumService(
        IRepository<Stadium> repository,
        IDistributedCache cache,
        ILogger<StadiumService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<StadiumDto>> GetAllAsync(
        string? city,
        string? country,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        // Build cache key from filters
        var cacheKey = $"{CacheKeyPrefix}list:{city}:{country}:{pagination.Page}:{pagination.PageSize}";

        // Try get from cache
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogDebug("Returning stadiums from cache: {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<PagedResult<StadiumDto>>(cachedData)!;
        }

        // Build filter predicate
        var predicate = BuildPredicate(city, country);

        // Get from database with pagination
        var pagedStadiums = await _repository.FindPagedAsync(predicate, pagination, cancellationToken);
        var dtos = pagedStadiums.Items.Select(MapToDto).ToList();

        var result = new PagedResult<StadiumDto>(
            dtos,
            pagedStadiums.Page,
            pagedStadiums.PageSize,
            pagedStadiums.TotalCount);

        // Cache the result
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            options,
            cancellationToken);

        _logger.LogDebug("Cached {Count} stadiums with key: {CacheKey}", dtos.Count, cacheKey);
        return result;
    }

    public async Task<StadiumDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";

        // Try get from cache
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogDebug("Returning stadium {Id} from cache", id);
            return JsonSerializer.Deserialize<StadiumDto>(cachedData);
        }

        // Get from database
        var stadium = await _repository.GetByIdAsync(id, cancellationToken);
        if (stadium == null)
        {
            return null;
        }

        var dto = MapToDto(stadium);

        // Cache the result
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(dto),
            options,
            cancellationToken);

        _logger.LogDebug("Cached stadium {Id}", id);
        return dto;
    }

    private static System.Linq.Expressions.Expression<Func<Stadium, bool>> BuildPredicate(
        string? city,
        string? country)
    {
        return s =>
            (string.IsNullOrEmpty(city) || s.City == city) &&
            (string.IsNullOrEmpty(country) || s.Country == country);
    }

    private static StadiumDto MapToDto(Stadium entity)
    {
        return new StadiumDto
        {
            Id = entity.Id,
            Name = entity.Name ?? string.Empty,
            City = entity.City ?? string.Empty,
            Country = entity.Country ?? string.Empty,
            Capacity = entity.Capacity,
            StadiumWiki = entity.StadiumWiki,
            CityWiki = entity.CityWiki
        };
    }
}
