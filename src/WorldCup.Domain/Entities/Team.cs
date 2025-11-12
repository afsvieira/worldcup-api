namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a national football team (e.g., Brazil, Germany, Argentina)
/// </summary>
public class Team
{
    public int Id { get; set; }
    public string? SourceTeamId { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public bool? IsMenTeam { get; set; }
    public bool? IsWomenTeam { get; set; }
    public string? Federation { get; set; }
    public string? Region { get; set; }
    public string? ConfederationId { get; set; }
    public string? Confederation { get; set; }
    public string? ConfederationCode { get; set; }
    public string? FederationWiki { get; set; }

    // Navigation properties
    public virtual ICollection<Match> HomeMatches { get; set; } = new List<Match>();
    public virtual ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Substitution> Substitutions { get; set; } = new List<Substitution>();
    public virtual ICollection<TournamentStanding> Standings { get; set; } = new List<TournamentStanding>();
    public virtual ICollection<AwardWinner> AwardWinners { get; set; } = new List<AwardWinner>();
}
