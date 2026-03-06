namespace BastionBuilder.Domain.Entities;

/// <summary>
/// A logical group of wall segments that form a single continuous wall
/// (e.g., the north wall of a courtyard, or a shared partition between two rooms).
/// </summary>
public class WallGroup
{
    /// <summary>Unique identifier for this wall group.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Display name of the wall group (e.g., "Outer North Wall").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description or notes about this wall group.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The wall segments that make up this group.</summary>
    public List<WallSegment> Segments { get; set; } = [];

    /// <summary>
    /// Optional floor level this wall group primarily belongs to.
    /// </summary>
    public int FloorLevel { get; set; }
}
