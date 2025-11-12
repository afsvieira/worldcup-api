namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a stadium where World Cup matches are played
/// </summary>
public class Stadium
{
    public int Id { get; set; }
    public string? SourceStadiumId { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public int? Capacity { get; set; }
    public string? StadiumWiki { get; set; }
    public string? CityWiki { get; set; }

    // Navigation properties
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
