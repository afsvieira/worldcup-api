namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a player or team who won a World Cup award
/// </summary>
public class AwardWinner
{
    public int Id { get; set; }
    public string? SourceTournamentId { get; set; }
    public string? SourceAwardId { get; set; }
    public string? SourcePlayerId { get; set; }
    public string? SourceTeamId { get; set; }
    public bool? Shared { get; set; }

    // Navigation properties
    public virtual Tournament? Tournament { get; set; }
    public virtual Award? Award { get; set; }
    public virtual Player? Player { get; set; }
    public virtual Team? Team { get; set; }
}
