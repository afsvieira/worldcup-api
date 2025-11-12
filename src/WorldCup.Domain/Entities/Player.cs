namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a football player who participated in World Cup tournaments
/// </summary>
public class Player
{
    public int Id { get; set; }
    public string? SourcePlayerId { get; set; }
    public string? FamilyName { get; set; }
    public string? GivenName { get; set; }
    public string? BirthDate { get; set; }
    public bool? IsFemale { get; set; }
    public bool? IsGoalkeeper { get; set; }
    public bool? IsDefender { get; set; }
    public bool? IsMidfielder { get; set; }
    public bool? IsForward { get; set; }
    public int? TournamentCount { get; set; }
    public string? TournamentList { get; set; }
    public string? WikiLink { get; set; }

    // Navigation properties
    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Substitution> SubstitutionsOn { get; set; } = new List<Substitution>();
    public virtual ICollection<AwardWinner> Awards { get; set; } = new List<AwardWinner>();

    // Computed property
    public string FullName => $"{GivenName} {FamilyName}".Trim();
}
