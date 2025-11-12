namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a yellow or red card shown to a player during a match
/// </summary>
public class Booking
{
    public int Id { get; set; }
    public string? SourceBookingId { get; set; }
    public string? SourceTournamentId { get; set; }
    public string? SourceMatchId { get; set; }
    public string? SourceTeamId { get; set; }
    public string? SourcePlayerId { get; set; }
    public string? MinuteLabel { get; set; }
    public int? MinuteRegulation { get; set; }
    public int? MinuteStoppage { get; set; }
    public string? MatchPeriod { get; set; }
    public bool? IsYellowCard { get; set; }
    public bool? IsRedCard { get; set; }
    public bool? IsSecondYellow { get; set; }
    public bool? IsSendingOff { get; set; }

    // Navigation properties
    public virtual Tournament? Tournament { get; set; }
    public virtual Match? Match { get; set; }
    public virtual Team? Team { get; set; }
    public virtual Player? Player { get; set; }
}
