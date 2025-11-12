namespace WorldCup.Domain.Enums;

/// <summary>
/// Match periods during a game
/// </summary>
public enum MatchPeriod
{
    /// <summary>
    /// First half of regular time
    /// </summary>
    FirstHalf,
    
    /// <summary>
    /// Second half of regular time
    /// </summary>
    SecondHalf,
    
    /// <summary>
    /// First half of extra time
    /// </summary>
    ExtraTimeFirstHalf,
    
    /// <summary>
    /// Second half of extra time
    /// </summary>
    ExtraTimeSecondHalf,
    
    /// <summary>
    /// Penalty shootout
    /// </summary>
    PenaltyShootout
}
