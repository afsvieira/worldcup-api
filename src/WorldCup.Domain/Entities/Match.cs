namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a World Cup match between two teams
/// </summary>
public class Match
{
    public int Id { get; set; }
    public string? SourceMatchId { get; set; }
    public string? SourceTournamentId { get; set; }
    public string? SourceStadiumId { get; set; }
    public string? SourceHomeTeamId { get; set; }
    public string? SourceAwayTeamId { get; set; }
    public string? Name { get; set; }
    public string? Stage { get; set; }
    public string? GroupName { get; set; }
    public string? MatchDateTime { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public int? HomePenalties { get; set; }
    public int? AwayPenalties { get; set; }
    public bool? HasExtraTime { get; set; }
    public bool? HasPenaltyShootout { get; set; }
    public string? Result { get; set; }
    public bool? HomeWin { get; set; }
    public bool? AwayWin { get; set; }
    public bool? IsDraw { get; set; }

    // Navigation properties
    public virtual Tournament? Tournament { get; set; }
    public virtual Stadium? Stadium { get; set; }
    public virtual Team? HomeTeam { get; set; }
    public virtual Team? AwayTeam { get; set; }
    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Substitution> Substitutions { get; set; } = new List<Substitution>();
}
