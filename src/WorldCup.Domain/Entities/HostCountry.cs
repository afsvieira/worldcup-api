namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a country that hosted a World Cup tournament (supports multi-host tournaments)
/// </summary>
public class HostCountry
{
    public int Id { get; set; }
    public string? SourceHostCountryId { get; set; }
    public string? SourceTournamentId { get; set; }
    public string? CountryName { get; set; }

    // Navigation properties
    public virtual Tournament? Tournament { get; set; }
}
