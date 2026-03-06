namespace BastionBuilder.Domain.Entities;

/// <summary>
/// Represents a room, chamber, corridor, or area in the bastion.
/// Nodes are connected by Edges (doors/windows/stairs) and bounded by WallSegments.
/// </summary>
public class Node
{
    /// <summary>Unique identifier for this node.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Display name of the node (e.g., "Great Hall", "Guard Post").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Full DM description of this location.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Public-facing description that players can observe.</summary>
    public string PublicDescription { get; set; } = string.Empty;

    /// <summary>
    /// Discoverability difficulty class for finding this node (0 = always visible, 1–20 = DC required).
    /// </summary>
    public int DiscoverabilityDC { get; set; }

    /// <summary>
    /// Ceiling height of this node in feet.
    /// Used as a stub for future height-scaling calculations.
    /// </summary>
    public double HeightFeet { get; set; } = 10.0;

    /// <summary>
    /// Floor area of the node in square feet.
    /// Used as a stub for future area-based calculations.
    /// </summary>
    public double AreaSquareFeet { get; set; }

    /// <summary>X position for map/canvas layout.</summary>
    public double PositionX { get; set; }

    /// <summary>Y position for map/canvas layout.</summary>
    public double PositionY { get; set; }

    /// <summary>Features and points of interest within this node.</summary>
    public List<Feature> Features { get; set; } = [];

    /// <summary>Floor number or level (0 = ground floor).</summary>
    public int FloorLevel { get; set; }

    /// <summary>Arbitrary tags for filtering or categorization.</summary>
    public List<string> Tags { get; set; } = [];
}
