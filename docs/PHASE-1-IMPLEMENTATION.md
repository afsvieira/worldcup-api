# WorldCup API - Phase 1 Implementation Plan

## 📋 Overview

**Phase 1: Foundation Endpoints**

This phase establishes the foundational patterns for the entire API by implementing the simplest, most cacheable endpoints:
- Competitions
- Teams
- Stadiums

**Goals:**
1. ✅ Establish DTO mapping patterns
2. ✅ Implement service layer with caching strategy
3. ✅ Create controllers with proper response formatting
4. ✅ Set up pagination and filtering
5. ✅ Configure Cache-Control headers
6. ✅ Add XML documentation
7. ✅ Write unit tests

**Why These Endpoints First?**
- Small datasets (2 competitions, 68 teams, 238 stadiums)
- No complex relationships
- Highly cacheable (24h TTL)
- Perfect for establishing patterns

---

## 🏗️ Architecture Pattern

```
Controller → Service → Repository → DbContext → Database
     ↓          ↓
    DTO   ← Mapping ←    Entity
```

**Layering:**
1. **Controller**: HTTP concerns, response formatting, validation
2. **Service**: Business logic, caching, DTO mapping
3. **Repository**: Data access, EF Core queries
4. **DTOs**: API response models (in Application layer)

---

## 📦 1. DTOs (Application Layer)

### 1.1 CompetitionDto

**File:** `src/WorldCup.Application/DTOs/CompetitionDto.cs`

```csharp
namespace WorldCup.Application.DTOs;

/// <summary>
/// Represents a FIFA competition (World Cup or Women's World Cup).
/// </summary>
public class CompetitionDto
{
    /// <summary>
    /// Unique competition identifier (e.g., "WC", "WCW").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Competition display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gender category.
    /// </summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Total number of tournaments held.
    /// </summary>
    public int TournamentCount { get; set; }
}
```

### 1.2 TeamDto

**File:** `src/WorldCup.Application/DTOs/TeamDto.cs`

```csharp
namespace WorldCup.Application.DTOs;

/// <summary>
/// Represents a national team.
/// </summary>
public class TeamDto
{
    /// <summary>
    /// Unique team identifier (e.g., "BRA", "GER", "ARG").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Team display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// FIFA confederation (e.g., "UEFA", "CONMEBOL").
    /// </summary>
    public string Confederation { get; set; } = string.Empty;

    /// <summary>
    /// Geographic region.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gender category.
    /// </summary>
    public string Gender { get; set; } = string.Empty;
}
```

### 1.3 StadiumDto

**File:** `src/WorldCup.Application/DTOs/StadiumDto.cs`

```csharp
namespace WorldCup.Application.DTOs;

/// <summary>
/// Represents a stadium venue.
/// </summary>
public class StadiumDto
{
    /// <summary>
    /// Unique stadium identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Stadium name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// City location.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Country location.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Seating capacity.
    /// </summary>
    public int? Capacity { get; set; }

    /// <summary>
    /// Latitude coordinate.
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate.
    /// </summary>
    public decimal? Longitude { get; set; }
}
```

### 1.4 ApiResponse<T>

**File:** `src/WorldCup.Application/DTOs/ApiResponse.cs`

```csharp
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
    /// Pagination links.
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
```

### 1.5 ErrorResponse

**File:** `src/WorldCup.Application/DTOs/ErrorResponse.cs`

```csharp
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
    /// Error code (e.g., "VALIDATION_ERROR", "NOT_FOUND").
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
```

---

## 🔧 2. Services (Application Layer)

### 2.1 ICompetitionService

**File:** `src/WorldCup.Application/Interfaces/ICompetitionService.cs`

```csharp
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Competition service interface.
/// </summary>
public interface ICompetitionService
{
    /// <summary>
    /// Gets all competitions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of competitions.</returns>
    Task<IEnumerable<CompetitionDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a competition by code.
    /// </summary>
    /// <param name="code">Competition code (e.g., "WC", "WCW").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Competition or null if not found.</returns>
    Task<CompetitionDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
```

### 2.2 CompetitionService

**File:** `src/WorldCup.Application/Services/CompetitionService.cs`

```csharp
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldCup.Application.DTOs;
using WorldCup.Application.Interfaces;
using WorldCup.Domain.Entities;

namespace WorldCup.Application.Services;

/// <summary>
/// Competition service implementation with caching.
/// </summary>
public class CompetitionService : ICompetitionService
{
    private readonly IRepository<Competition> _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CompetitionService> _logger;
    private const string CacheKeyAll = "competitions:all";
    private const string CacheKeyPrefix = "competitions:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public CompetitionService(
        IRepository<Competition> repository,
        IDistributedCache cache,
        ILogger<CompetitionService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<CompetitionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Try get from cache
        var cachedData = await _cache.GetStringAsync(CacheKeyAll, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogDebug("Returning competitions from cache");
            return JsonSerializer.Deserialize<IEnumerable<CompetitionDto>>(cachedData)!;
        }

        // Get from database
        var competitions = await _repository.GetAllAsync(cancellationToken);
        var dtos = competitions.Select(MapToDto).ToList();

        // Cache the result
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };
        await _cache.SetStringAsync(
            CacheKeyAll,
            JsonSerializer.Serialize(dtos),
            options,
            cancellationToken);

        _logger.LogDebug("Cached {Count} competitions", dtos.Count);
        return dtos;
    }

    public async Task<CompetitionDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{code}";

        // Try get from cache
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogDebug("Returning competition {Code} from cache", code);
            return JsonSerializer.Deserialize<CompetitionDto>(cachedData);
        }

        // Get from database
        var competition = await _repository.GetByIdAsync(code, cancellationToken);
        if (competition == null)
        {
            return null;
        }

        var dto = MapToDto(competition);

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

        return dto;
    }

    private static CompetitionDto MapToDto(Competition entity)
    {
        return new CompetitionDto
        {
            Code = entity.Code,
            Name = entity.Name,
            Gender = entity.Gender.ToString().ToLowerInvariant(),
            TournamentCount = entity.TournamentCount
        };
    }
}
```

### 2.3 ITeamService

**File:** `src/WorldCup.Application/Interfaces/ITeamService.cs`

```csharp
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Team service interface.
/// </summary>
public interface ITeamService
{
    /// <summary>
    /// Gets teams with optional filtering and pagination.
    /// </summary>
    /// <param name="confederation">Filter by confederation (optional).</param>
    /// <param name="region">Filter by region (optional).</param>
    /// <param name="gender">Filter by gender (optional).</param>
    /// <param name="pagination">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged team results.</returns>
    Task<PagedResult<TeamDto>> GetAllAsync(
        string? confederation,
        string? region,
        string? gender,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a team by code.
    /// </summary>
    /// <param name="code">Team code (e.g., "BRA", "GER").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Team or null if not found.</returns>
    Task<TeamDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches teams by name.
    /// </summary>
    /// <param name="name">Search term.</param>
    /// <param name="pagination">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged team results.</returns>
    Task<PagedResult<TeamDto>> SearchByNameAsync(
        string name,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);
}
```

### 2.4 TeamService

**File:** `src/WorldCup.Application/Services/TeamService.cs`

```csharp
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;
using WorldCup.Application.Interfaces;
using WorldCup.Domain.Entities;

namespace WorldCup.Application.Services;

/// <summary>
/// Team service implementation with caching.
/// </summary>
public class TeamService : ITeamService
{
    private readonly ITeamRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<TeamService> _logger;
    private const string CacheKeyPrefix = "teams:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public TeamService(
        ITeamRepository repository,
        IDistributedCache cache,
        ILogger<TeamService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<TeamDto>> GetAllAsync(
        string? confederation,
        string? region,
        string? gender,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        // Build cache key from filters
        var cacheKey = $"{CacheKeyPrefix}list:{confederation}:{region}:{gender}:{pagination.Page}:{pagination.PageSize}";

        // Try get from cache
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogDebug("Returning teams from cache");
            return JsonSerializer.Deserialize<PagedResult<TeamDto>>(cachedData)!;
        }

        // Build filter predicate
        var predicate = BuildPredicate(confederation, region, gender);

        // Get from database with pagination
        var pagedTeams = await _repository.FindPagedAsync(predicate, pagination, cancellationToken);
        var dtos = pagedTeams.Items.Select(MapToDto).ToList();

        var result = new PagedResult<TeamDto>
        {
            Items = dtos,
            Page = pagedTeams.Page,
            PageSize = pagedTeams.PageSize,
            TotalCount = pagedTeams.TotalCount
        };

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

        _logger.LogDebug("Cached {Count} teams", dtos.Count);
        return result;
    }

    public async Task<TeamDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{code}";

        // Try get from cache
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogDebug("Returning team {Code} from cache", code);
            return JsonSerializer.Deserialize<TeamDto>(cachedData);
        }

        // Get from database
        var team = await _repository.GetByIdAsync(code, cancellationToken);
        if (team == null)
        {
            return null;
        }

        var dto = MapToDto(team);

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

        return dto;
    }

    public async Task<PagedResult<TeamDto>> SearchByNameAsync(
        string name,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var pagedTeams = await _repository.SearchByNameAsync(name, pagination, cancellationToken);
        var dtos = pagedTeams.Items.Select(MapToDto).ToList();

        return new PagedResult<TeamDto>
        {
            Items = dtos,
            Page = pagedTeams.Page,
            PageSize = pagedTeams.PageSize,
            TotalCount = pagedTeams.TotalCount
        };
    }

    private static System.Linq.Expressions.Expression<Func<Team, bool>> BuildPredicate(
        string? confederation,
        string? region,
        string? gender)
    {
        return t =>
            (string.IsNullOrEmpty(confederation) || t.Confederation == confederation) &&
            (string.IsNullOrEmpty(region) || t.Region == region) &&
            (string.IsNullOrEmpty(gender) || t.Gender.ToString().ToLower() == gender.ToLower());
    }

    private static TeamDto MapToDto(Team entity)
    {
        return new TeamDto
        {
            Code = entity.Code,
            Name = entity.Name,
            Confederation = entity.Confederation,
            Region = entity.Region,
            Gender = entity.Gender.ToString().ToLowerInvariant()
        };
    }
}
```

### 2.5 IStadiumService

**File:** `src/WorldCup.Application/Interfaces/IStadiumService.cs`

```csharp
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;

namespace WorldCup.Application.Interfaces;

/// <summary>
/// Stadium service interface.
/// </summary>
public interface IStadiumService
{
    /// <summary>
    /// Gets stadiums with pagination.
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
```

### 2.6 StadiumService

**File:** `src/WorldCup.Application/Services/StadiumService.cs`

```csharp
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;
using WorldCup.Application.Interfaces;
using WorldCup.Domain.Entities;

namespace WorldCup.Application.Services;

/// <summary>
/// Stadium service implementation with caching.
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
            _logger.LogDebug("Returning stadiums from cache");
            return JsonSerializer.Deserialize<PagedResult<StadiumDto>>(cachedData)!;
        }

        // Build filter predicate
        var predicate = BuildPredicate(city, country);

        // Get from database with pagination
        var pagedStadiums = await _repository.FindPagedAsync(predicate, pagination, cancellationToken);
        var dtos = pagedStadiums.Items.Select(MapToDto).ToList();

        var result = new PagedResult<StadiumDto>
        {
            Items = dtos,
            Page = pagedStadiums.Page,
            PageSize = pagedStadiums.PageSize,
            TotalCount = pagedStadiums.TotalCount
        };

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

        _logger.LogDebug("Cached {Count} stadiums", dtos.Count);
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
            Name = entity.Name,
            City = entity.City,
            Country = entity.Country,
            Capacity = entity.Capacity,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude
        };
    }
}
```

### 2.7 Service Registration

**File:** `src/WorldCup.Application/Extensions/ServiceExtensions.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using WorldCup.Application.Interfaces;
using WorldCup.Application.Services;

namespace WorldCup.Application.Extensions;

/// <summary>
/// Service registration extensions.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers application services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICompetitionService, CompetitionService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IStadiumService, StadiumService>();

        return services;
    }
}
```

---

## 🎮 3. Controllers (API Layer)

### 3.1 CompetitionsController

**File:** `src/WorldCup.API/Controllers/CompetitionsController.cs`

```csharp
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WorldCup.Application.DTOs;
using WorldCup.Application.Interfaces;

namespace WorldCup.API.Controllers;

/// <summary>
/// Competition endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CompetitionsController : ControllerBase
{
    private readonly ICompetitionService _service;
    private readonly ILogger<CompetitionsController> _logger;

    public CompetitionsController(
        ICompetitionService service,
        ILogger<CompetitionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all FIFA competitions.
    /// </summary>
    /// <remarks>
    /// Returns a list of all FIFA competitions (World Cup and Women's World Cup).
    /// This endpoint is heavily cached (24 hours).
    /// </remarks>
    /// <response code="200">List of competitions.</response>
    [HttpGet]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { })]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompetitionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompetitionDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var competitions = await _service.GetAllAsync(cancellationToken);

        var response = new ApiResponse<IEnumerable<CompetitionDto>>
        {
            Data = competitions,
            Meta = new ApiResponseMeta
            {
                Timestamp = DateTime.UtcNow.ToString("o")
            }
        };

        // Set Cache-Control header
        Response.Headers.Append("Cache-Control", "public, max-age=86400");

        return Ok(response);
    }

    /// <summary>
    /// Gets a competition by code.
    /// </summary>
    /// <param name="code">Competition code (e.g., "WC", "WCW").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Returns details of a specific FIFA competition.
    /// This endpoint is heavily cached (24 hours).
    /// </remarks>
    /// <response code="200">Competition details.</response>
    /// <response code="404">Competition not found.</response>
    [HttpGet("{code}")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "code" })]
    [ProducesResponseType(typeof(ApiResponse<CompetitionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CompetitionDto>>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var competition = await _service.GetByCodeAsync(code, cancellationToken);
        
        if (competition == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "NOT_FOUND",
                    Message = $"Competition with code '{code}' not found.",
                    StatusCode = 404
                }
            });
        }

        var response = new ApiResponse<CompetitionDto>
        {
            Data = competition,
            Meta = new ApiResponseMeta
            {
                Timestamp = DateTime.UtcNow.ToString("o")
            }
        };

        // Set Cache-Control header
        Response.Headers.Append("Cache-Control", "public, max-age=86400");

        return Ok(response);
    }
}
```

### 3.2 TeamsController

**File:** `src/WorldCup.API/Controllers/TeamsController.cs`

```csharp
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;
using WorldCup.Application.Interfaces;

namespace WorldCup.API.Controllers;

/// <summary>
/// Team endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _service;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(
        ITeamService service,
        ILogger<TeamsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all teams with optional filtering and pagination.
    /// </summary>
    /// <param name="confederation">Filter by confederation (e.g., "UEFA", "CONMEBOL").</param>
    /// <param name="region">Filter by region.</param>
    /// <param name="gender">Filter by gender ("male" or "female").</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10, max: 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Returns a paginated list of teams with optional filters.
    /// This endpoint is heavily cached (24 hours).
    /// 
    /// Example: GET /api/v1/teams?confederation=UEFA&amp;gender=male&amp;page=1&amp;pageSize=20
    /// </remarks>
    /// <response code="200">Paginated list of teams.</response>
    /// <response code="400">Invalid query parameters.</response>
    [HttpGet]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "confederation", "region", "gender", "page", "pageSize" })]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TeamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetAll(
        [FromQuery] string? confederation,
        [FromQuery] string? region,
        [FromQuery] string? gender,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Validate gender parameter
        if (!string.IsNullOrEmpty(gender) && 
            !gender.Equals("male", StringComparison.OrdinalIgnoreCase) && 
            !gender.Equals("female", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Invalid gender value. Must be 'male' or 'female'.",
                    StatusCode = 400,
                    Details = new List<ErrorFieldDetail>
                    {
                        new() { Field = "gender", Issue = $"Invalid value '{gender}'" }
                    }
                }
            });
        }

        // Validate pagination
        if (page < 1)
        {
            return BadRequest(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Page number must be >= 1.",
                    StatusCode = 400,
                    Details = new List<ErrorFieldDetail>
                    {
                        new() { Field = "page", Issue = $"Invalid value {page}" }
                    }
                }
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Page size must be between 1 and 100.",
                    StatusCode = 400,
                    Details = new List<ErrorFieldDetail>
                    {
                        new() { Field = "pageSize", Issue = $"Invalid value {pageSize}" }
                    }
                }
            });
        }

        var pagination = new PaginationParameters { Page = page, PageSize = pageSize };
        var result = await _service.GetAllAsync(confederation, region, gender, pagination, cancellationToken);

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
        var queryString = BuildQueryString(confederation, region, gender);

        var response = new ApiResponse<IEnumerable<TeamDto>>
        {
            Data = result.Items,
            Meta = new ApiResponseMeta
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            },
            Links = new ApiResponseLinks
            {
                Self = $"{baseUrl}?{queryString}&page={page}&pageSize={pageSize}",
                First = $"{baseUrl}?{queryString}&page=1&pageSize={pageSize}",
                Last = $"{baseUrl}?{queryString}&page={result.TotalPages}&pageSize={pageSize}",
                Next = result.HasNextPage ? $"{baseUrl}?{queryString}&page={page + 1}&pageSize={pageSize}" : null,
                Prev = result.HasPreviousPage ? $"{baseUrl}?{queryString}&page={page - 1}&pageSize={pageSize}" : null
            }
        };

        // Set Cache-Control header
        Response.Headers.Append("Cache-Control", "public, max-age=86400");

        return Ok(response);
    }

    /// <summary>
    /// Gets a team by code.
    /// </summary>
    /// <param name="code">Team code (e.g., "BRA", "GER", "ARG").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Returns details of a specific national team.
    /// This endpoint is heavily cached (24 hours).
    /// </remarks>
    /// <response code="200">Team details.</response>
    /// <response code="404">Team not found.</response>
    [HttpGet("{code}")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "code" })]
    [ProducesResponseType(typeof(ApiResponse<TeamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TeamDto>>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var team = await _service.GetByCodeAsync(code, cancellationToken);
        
        if (team == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "NOT_FOUND",
                    Message = $"Team with code '{code}' not found.",
                    StatusCode = 404
                }
            });
        }

        var response = new ApiResponse<TeamDto>
        {
            Data = team,
            Meta = new ApiResponseMeta
            {
                Timestamp = DateTime.UtcNow.ToString("o")
            }
        };

        // Set Cache-Control header
        Response.Headers.Append("Cache-Control", "public, max-age=86400");

        return Ok(response);
    }

    /// <summary>
    /// Searches teams by name.
    /// </summary>
    /// <param name="name">Search term.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10, max: 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Performs a case-insensitive partial match search on team names.
    /// 
    /// Example: GET /api/v1/teams/search?name=brazil&amp;page=1&amp;pageSize=10
    /// </remarks>
    /// <response code="200">Paginated search results.</response>
    /// <response code="400">Invalid query parameters.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TeamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> Search(
        [FromQuery] string name,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Search name parameter is required.",
                    StatusCode = 400
                }
            });
        }

        var pagination = new PaginationParameters { Page = page, PageSize = pageSize };
        var result = await _service.SearchByNameAsync(name, pagination, cancellationToken);

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var response = new ApiResponse<IEnumerable<TeamDto>>
        {
            Data = result.Items,
            Meta = new ApiResponseMeta
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            },
            Links = new ApiResponseLinks
            {
                Self = $"{baseUrl}?name={name}&page={page}&pageSize={pageSize}",
                First = $"{baseUrl}?name={name}&page=1&pageSize={pageSize}",
                Last = $"{baseUrl}?name={name}&page={result.TotalPages}&pageSize={pageSize}",
                Next = result.HasNextPage ? $"{baseUrl}?name={name}&page={page + 1}&pageSize={pageSize}" : null,
                Prev = result.HasPreviousPage ? $"{baseUrl}?name={name}&page={page - 1}&pageSize={pageSize}" : null
            }
        };

        return Ok(response);
    }

    private static string BuildQueryString(string? confederation, string? region, string? gender)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(confederation)) parts.Add($"confederation={confederation}");
        if (!string.IsNullOrEmpty(region)) parts.Add($"region={region}");
        if (!string.IsNullOrEmpty(gender)) parts.Add($"gender={gender}");
        return string.Join("&", parts);
    }
}
```

### 3.3 StadiumsController

**File:** `src/WorldCup.API/Controllers/StadiumsController.cs`

```csharp
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WorldCup.Application.Common.Models;
using WorldCup.Application.DTOs;
using WorldCup.Application.Interfaces;

namespace WorldCup.API.Controllers;

/// <summary>
/// Stadium endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class StadiumsController : ControllerBase
{
    private readonly IStadiumService _service;
    private readonly ILogger<StadiumsController> _logger;

    public StadiumsController(
        IStadiumService service,
        ILogger<StadiumsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all stadiums with optional filtering and pagination.
    /// </summary>
    /// <param name="city">Filter by city.</param>
    /// <param name="country">Filter by country.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10, max: 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Returns a paginated list of stadiums with optional filters.
    /// This endpoint is heavily cached (24 hours).
    /// 
    /// Example: GET /api/v1/stadiums?country=Brazil&amp;page=1&amp;pageSize=20
    /// </remarks>
    /// <response code="200">Paginated list of stadiums.</response>
    /// <response code="400">Invalid query parameters.</response>
    [HttpGet]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "city", "country", "page", "pageSize" })]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StadiumDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StadiumDto>>>> GetAll(
        [FromQuery] string? city,
        [FromQuery] string? country,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Validate pagination
        if (page < 1)
        {
            return BadRequest(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Page number must be >= 1.",
                    StatusCode = 400
                }
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "VALIDATION_ERROR",
                    Message = "Page size must be between 1 and 100.",
                    StatusCode = 400
                }
            });
        }

        var pagination = new PaginationParameters { Page = page, PageSize = pageSize };
        var result = await _service.GetAllAsync(city, country, pagination, cancellationToken);

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
        var queryString = BuildQueryString(city, country);

        var response = new ApiResponse<IEnumerable<StadiumDto>>
        {
            Data = result.Items,
            Meta = new ApiResponseMeta
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            },
            Links = new ApiResponseLinks
            {
                Self = $"{baseUrl}?{queryString}&page={page}&pageSize={pageSize}",
                First = $"{baseUrl}?{queryString}&page=1&pageSize={pageSize}",
                Last = $"{baseUrl}?{queryString}&page={result.TotalPages}&pageSize={pageSize}",
                Next = result.HasNextPage ? $"{baseUrl}?{queryString}&page={page + 1}&pageSize={pageSize}" : null,
                Prev = result.HasPreviousPage ? $"{baseUrl}?{queryString}&page={page - 1}&pageSize={pageSize}" : null
            }
        };

        // Set Cache-Control header
        Response.Headers.Append("Cache-Control", "public, max-age=86400");

        return Ok(response);
    }

    /// <summary>
    /// Gets a stadium by ID.
    /// </summary>
    /// <param name="id">Stadium ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Returns details of a specific stadium venue.
    /// This endpoint is heavily cached (24 hours).
    /// </remarks>
    /// <response code="200">Stadium details.</response>
    /// <response code="404">Stadium not found.</response>
    [HttpGet("{id:int}")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "id" })]
    [ProducesResponseType(typeof(ApiResponse<StadiumDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StadiumDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var stadium = await _service.GetByIdAsync(id, cancellationToken);
        
        if (stadium == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "NOT_FOUND",
                    Message = $"Stadium with ID {id} not found.",
                    StatusCode = 404
                }
            });
        }

        var response = new ApiResponse<StadiumDto>
        {
            Data = stadium,
            Meta = new ApiResponseMeta
            {
                Timestamp = DateTime.UtcNow.ToString("o")
            }
        };

        // Set Cache-Control header
        Response.Headers.Append("Cache-Control", "public, max-age=86400");

        return Ok(response);
    }

    private static string BuildQueryString(string? city, string? country)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(city)) parts.Add($"city={city}");
        if (!string.IsNullOrEmpty(country)) parts.Add($"country={country}");
        return string.Join("&", parts);
    }
}
```

---

## 🧪 4. Testing Strategy

### 4.1 Unit Tests

**File:** `tests/WorldCup.Application.Tests/Services/CompetitionServiceTests.cs`

```csharp
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using WorldCup.Application.Interfaces;
using WorldCup.Application.Services;
using WorldCup.Domain.Entities;
using WorldCup.Domain.Enums;
using Xunit;

namespace WorldCup.Application.Tests.Services;

public class CompetitionServiceTests
{
    private readonly Mock<IRepository<Competition>> _repositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<ILogger<CompetitionService>> _loggerMock;
    private readonly CompetitionService _service;

    public CompetitionServiceTests()
    {
        _repositoryMock = new Mock<IRepository<Competition>>();
        _cacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<CompetitionService>>();
        _service = new CompetitionService(_repositoryMock.Object, _cacheMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCompetitions()
    {
        // Arrange
        var competitions = new List<Competition>
        {
            new() { Code = "WC", Name = "FIFA World Cup", Gender = Gender.Male, TournamentCount = 22 },
            new() { Code = "WCW", Name = "FIFA Women's World Cup", Gender = Gender.Female, TournamentCount = 9 }
        };

        _cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync((byte[]?)null);

        _repositoryMock.Setup(x => x.GetAllAsync(default))
            .ReturnsAsync(competitions);

        _cacheMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Code == "WC");
        Assert.Contains(result, c => c.Code == "WCW");
    }

    [Fact]
    public async Task GetByCodeAsync_ExistingCode_ReturnsCompetition()
    {
        // Arrange
        var competition = new Competition 
        { 
            Code = "WC", 
            Name = "FIFA World Cup", 
            Gender = Gender.Male, 
            TournamentCount = 22 
        };

        _cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync((byte[]?)null);

        _repositoryMock.Setup(x => x.GetByIdAsync("WC", default))
            .ReturnsAsync(competition);

        // Act
        var result = await _service.GetByCodeAsync("WC");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("WC", result.Code);
        Assert.Equal("FIFA World Cup", result.Name);
    }

    [Fact]
    public async Task GetByCodeAsync_NonExistingCode_ReturnsNull()
    {
        // Arrange
        _cacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync((byte[]?)null);

        _repositoryMock.Setup(x => x.GetByIdAsync("INVALID", default))
            .ReturnsAsync((Competition?)null);

        // Act
        var result = await _service.GetByCodeAsync("INVALID");

        // Assert
        Assert.Null(result);
    }
}
```

### 4.2 Integration Tests

**File:** `tests/WorldCup.API.Tests/Controllers/CompetitionsControllerTests.cs`

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using WorldCup.Application.DTOs;
using Xunit;

namespace WorldCup.API.Tests.Controllers;

public class CompetitionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CompetitionsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessAndCompetitions()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/competitions");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CompetitionDto>>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetByCode_ExistingCode_ReturnsCompetition()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/competitions/WC");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CompetitionDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal("WC", result.Data.Code);
    }

    [Fact]
    public async Task GetByCode_NonExistingCode_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/competitions/INVALID");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
```

---

## 📝 5. Implementation Checklist

### Phase 1.1: DTOs & Response Models
- [ ] Create `CompetitionDto.cs`
- [ ] Create `TeamDto.cs`
- [ ] Create `StadiumDto.cs`
- [ ] Create `ApiResponse.cs`
- [ ] Create `ErrorResponse.cs`

### Phase 1.2: Service Layer
- [ ] Create `ICompetitionService.cs`
- [ ] Create `CompetitionService.cs` with caching
- [ ] Create `ITeamService.cs`
- [ ] Create `TeamService.cs` with caching
- [ ] Create `IStadiumService.cs`
- [ ] Create `StadiumService.cs` with caching
- [ ] Create `ServiceExtensions.cs` for DI

### Phase 1.3: Controller Layer
- [ ] Create `CompetitionsController.cs`
- [ ] Create `TeamsController.cs`
- [ ] Create `StadiumsController.cs`
- [ ] Add XML documentation to all endpoints
- [ ] Configure `ResponseCache` attributes

### Phase 1.4: Redis Configuration
- [ ] Add `StackExchange.Redis` NuGet package
- [ ] Add Redis connection string to user secrets
- [ ] Configure `AddStackExchangeRedisCache` in `Program.cs`
- [ ] Test caching with Redis

### Phase 1.5: Program.cs Updates
- [ ] Add `builder.Services.AddApplicationServices()`
- [ ] Add Redis cache configuration
- [ ] Add response caching middleware
- [ ] Configure Swagger for new endpoints

### Phase 1.6: Testing
- [ ] Write unit tests for `CompetitionService`
- [ ] Write unit tests for `TeamService`
- [ ] Write unit tests for `StadiumService`
- [ ] Write integration tests for `CompetitionsController`
- [ ] Write integration tests for `TeamsController`
- [ ] Write integration tests for `StadiumsController`

### Phase 1.7: Documentation & Validation
- [ ] Test all endpoints in Swagger
- [ ] Verify caching headers in responses
- [ ] Verify pagination works correctly
- [ ] Verify filtering works correctly
- [ ] Verify error responses follow standard format
- [ ] Update README with Phase 1 endpoints

---

## 🚀 6. Next Steps After Phase 1

Once Phase 1 is complete and tested:

1. **Phase 2**: Tournament endpoints (with includes)
2. **Phase 3**: Match endpoints (with events)
3. **Phase 4**: Player endpoints (with search)
4. **Phase 5**: Advanced tournament features (summary, bracket, etc.)
5. **Phase 6**: Statistics and premium endpoints

---

## 📊 7. Expected Outcomes

After completing Phase 1, you will have:

✅ **Working Endpoints**:
- `GET /api/v1/competitions`
- `GET /api/v1/competitions/{code}`
- `GET /api/v1/teams`
- `GET /api/v1/teams/{code}`
- `GET /api/v1/teams/search?name={query}`
- `GET /api/v1/stadiums`
- `GET /api/v1/stadiums/{id}`

✅ **Established Patterns**:
- DTO mapping from entities
- Service layer with Redis caching
- Controller response formatting
- Pagination with links
- Error handling
- XML documentation

✅ **Infrastructure**:
- Redis caching configured
- Response caching headers
- Swagger documentation
- Unit and integration tests

This foundation will make implementing subsequent phases much faster and more consistent! 🎉
