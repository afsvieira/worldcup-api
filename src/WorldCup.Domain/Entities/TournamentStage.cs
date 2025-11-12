namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a stage of a tournament (e.g., Group Stage, Round of 16, Quarter-Finals, Semi-Finals, Final)
/// </summary>
public class TournamentStage
{
    public int Id { get; set; }
    public string? SourceTournamentId { get; set; }
    public int? StageNumber { get; set; }
    public string? StageName { get; set; }
    public bool? IsGroupStage { get; set; }
    public bool? IsKnockoutStage { get; set; }
    public bool? HasUnbalancedGroups { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public int? MatchCount { get; set; }
    public int? TeamCount { get; set; }
    public int? ScheduledCount { get; set; }
    public int? ReplayCount { get; set; }
    public int? PlayoffCount { get; set; }
    public int? WalkoverCount { get; set; }

    // Navigation properties
    public virtual Tournament? Tournament { get; set; }
}
