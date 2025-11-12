namespace WorldCup.Domain.Entities;

/// <summary>
/// Represents a World Cup award (e.g., Golden Ball, Golden Boot, Golden Glove)
/// </summary>
public class Award
{
    public int Id { get; set; }
    public string? SourceAwardId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<AwardWinner> Winners { get; set; } = new List<AwardWinner>();
}
