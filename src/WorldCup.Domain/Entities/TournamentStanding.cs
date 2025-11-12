namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a team's final position/ranking in a tournament
/// </summary>
public class TournamentStanding
{
    public int Id { get; set; }
    public string? SourceTournamentId { get; set; }
    public string? SourceTeamId { get; set; }
    public int? Position { get; set; }

    // Navigation properties
    public virtual Tournament? Tournament { get; set; }
    public virtual Team? Team { get; set; }
}
