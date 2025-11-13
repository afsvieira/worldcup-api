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
    /// Wikipedia link.
    /// </summary>
    public string? StadiumWiki { get; set; }

    /// <summary>
    /// City Wikipedia link.
    /// </summary>
    public string? CityWiki { get; set; }
}
