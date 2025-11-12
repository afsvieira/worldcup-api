namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a specific World Cup tournament held in a particular year (e.g., 2022 FIFA World Cup in Qatar)
/// </summary>
public class Tournament
{
    /// <summary>
    /// Unique identifier for the tournament
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// External source identifier for the tournament
    /// </summary>
    public string? SourceTournamentId { get; set; }
    
    /// <summary>
    /// Foreign key to the Competition
    /// </summary>
    public int? CompetitionId { get; set; }
    
    /// <summary>
    /// Name of the tournament (e.g., "2022 FIFA World Cup")
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Year the tournament was held
    /// </summary>
    public int? Year { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? HostCountry { get; set; }
    public string? WinnerTeamName { get; set; }
    public bool? HostWon { get; set; }
    public int? TeamCount { get; set; }
    public bool? HasGroupStage { get; set; }
    public bool? HasSecondGroupStage { get; set; }
    public bool? HasFinalRound { get; set; }
    public bool? HasRoundOf16 { get; set; }
    public bool? HasQuarterFinals { get; set; }
    public bool? HasSemiFinals { get; set; }
    public bool? HasThirdPlaceMatch { get; set; }
    public bool? HasFinal { get; set; }

    // Navigation properties
    public virtual Competition? Competition { get; set; }
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    public virtual ICollection<TournamentStage> Stages { get; set; } = new List<TournamentStage>();
    public virtual ICollection<TournamentStanding> Standings { get; set; } = new List<TournamentStanding>();
    public virtual ICollection<HostCountry> HostCountries { get; set; } = new List<HostCountry>();
    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Substitution> Substitutions { get; set; } = new List<Substitution>();
    public virtual ICollection<AwardWinner> AwardWinners { get; set; } = new List<AwardWinner>();
}
