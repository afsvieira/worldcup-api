namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a goal scored in a World Cup match
/// </summary>
public class Goal
{
    public int Id { get; set; }
    public string? SourceGoalId { get; set; }
    public string? SourceTournamentId { get; set; }
    public string? SourceMatchId { get; set; }
    public string? SourceTeamId { get; set; }
    public string? SourcePlayerId { get; set; }
    public string? MinuteLabel { get; set; }
    public int? MinuteRegulation { get; set; }
    public int? MinuteStoppage { get; set; }
    public string? MatchPeriod { get; set; }
    public bool? IsOwnGoal { get; set; }
    public bool? IsPenalty { get; set; }

    // Navigation properties
    public virtual Tournament? Tournament { get; set; }
    public virtual Match? Match { get; set; }
    public virtual Team? Team { get; set; }
    public virtual Player? Player { get; set; }
}
