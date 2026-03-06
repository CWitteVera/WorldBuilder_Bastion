namespace BastionBuilder.Domain.Entities;

/// <summary>
/// The Bastion aggregate root.  All nodes, edges and wall groups that make up
/// a single fortification are owned by one Bastion instance.
/// </summary>
public class Bastion
{
    /// <summary>Unique identifier for this bastion.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Display name of the bastion.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>General notes or lore about this bastion.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Date/time this record was created (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Date/time this record was last modified (UTC).</summary>
    public DateTime ModifiedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>All rooms/areas that make up this bastion.</summary>
    public List<Node> Nodes { get; set; } = [];

    /// <summary>All passages (doors, windows, stairs, etc.) connecting nodes.</summary>
    public List<Edge> Edges { get; set; } = [];

    /// <summary>All wall groups (collections of wall segments) in this bastion.</summary>
    public List<WallGroup> WallGroups { get; set; } = [];
}
