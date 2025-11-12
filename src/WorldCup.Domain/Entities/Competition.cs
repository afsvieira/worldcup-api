namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a FIFA competition (e.g., FIFA World Cup, FIFA Women's World Cup)
/// </summary>
public class Competition
{
    /// <summary>
    /// Unique identifier for the competition
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Competition code (e.g., "WC" for World Cup)
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Full name of the competition
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gender of the competition (Male/Female/Mixed)
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Collection of tournaments belonging to this competition
    /// </summary>
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
