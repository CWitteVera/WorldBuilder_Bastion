namespace BastionBuilder.Domain.Entities;

/// <summary>
/// Represents a portion of a wall segment that has been opened (e.g., an arrow slit, window
/// opening, or architectural gap). Opening splits reduce the effective area of the segment.
/// </summary>
public class OpeningSplit
{
    /// <summary>Unique identifier for this opening split.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>The wall segment this opening belongs to.</summary>
    public Guid WallSegmentId { get; set; }

    /// <summary>Description of the opening (e.g., "arrow slit", "portcullis gap").</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Fraction of the wall segment's length occupied by this opening (0.0–1.0).
    /// Multiple openings may exist; total fraction should not exceed 1.0.
    /// </summary>
    public double SplitPercentage { get; set; }

    /// <summary>
    /// Optional edge ID if this opening corresponds to a door, window, or passage.
    /// </summary>
    public Guid? EdgeId { get; set; }
}
