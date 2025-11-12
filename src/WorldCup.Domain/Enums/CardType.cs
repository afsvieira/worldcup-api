namespace WorldCup.Domain.Enums;

/// <summary>
/// Types of cards shown to players
/// </summary>
public enum CardType
{
    /// <summary>
    /// Yellow card (warning)
    /// </summary>
    Yellow,
    
    /// <summary>
    /// Red card (sending off)
    /// </summary>
    Red,
    
    /// <summary>
    /// Second yellow card (results in red)
    /// </summary>
    SecondYellow
}
