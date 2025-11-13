using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WorldCup.API.Filters;
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
    [ValidatePagination]
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
        if (!string.IsNullOrEmpty(city)) parts.Add($"city={Uri.EscapeDataString(city)}");
        if (!string.IsNullOrEmpty(country)) parts.Add($"country={Uri.EscapeDataString(country)}");
        return string.Join("&", parts);
    }
}
