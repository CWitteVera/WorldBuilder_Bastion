namespace BastionBuilder.Domain.Entities;

/// <summary>
/// A wall segment defines one physical wall face connecting two nodes.
/// It belongs to a WallGroup and can be shared between adjacent rooms.
/// </summary>
public class WallSegment
{
    /// <summary>Unique identifier for this wall segment.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>The WallGroup this segment belongs to.</summary>
    public Guid WallGroupId { get; set; }

    /// <summary>The node on the "from" side of this wall.</summary>
    public Guid FromNodeId { get; set; }

    /// <summary>The node on the "to" side of this wall (may equal FromNodeId for outer walls).</summary>
    public Guid? ToNodeId { get; set; }

    /// <summary>
    /// Additional node IDs that share this wall segment (e.g., corner walls touching three rooms).
    /// </summary>
    public List<Guid> SharedWithNodeIds { get; set; } = [];

    /// <summary>Wall thickness in inches.</summary>
    public double ThicknessInches { get; set; } = 12.0;

    /// <summary>Primary construction material (e.g., "Stone", "Wood", "Iron").</summary>
    public string Material { get; set; } = string.Empty;

    /// <summary>Base armor class of the wall segment before reinforcements.</summary>
    public int BaseAC { get; set; } = 15;

    /// <summary>Base hit points of the wall segment before reinforcements.</summary>
    public int BaseHP { get; set; } = 27;

    /// <summary>
    /// Wall height in feet.
    /// Used as a stub for height-scaling damage/cover calculations.
    /// </summary>
    public double HeightFeet { get; set; } = 10.0;

    /// <summary>Wall length in feet.</summary>
    public double LengthFeet { get; set; }

    /// <summary>Additive reinforcements applied to this wall segment.</summary>
    public List<Reinforcement> Reinforcements { get; set; } = [];

    /// <summary>Openings (arrow slits, windows, portcullis gaps) in this segment.</summary>
    public List<OpeningSplit> OpeningSplits { get; set; } = [];
}
