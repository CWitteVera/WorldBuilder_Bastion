using BastionBuilder.Domain.Enums;

namespace BastionBuilder.Domain.Entities;

/// <summary>
/// Represents a discoverable feature or point of interest within a node or on an edge.
/// Supports secret visibility states and reveal propagation to linked features.
/// </summary>
public class Feature
{
    /// <summary>Unique identifier for this feature.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Display name of the feature.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Full description visible to the Dungeon Master.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Public-facing description shown when discovered by players.</summary>
    public string PublicDescription { get; set; } = string.Empty;

    /// <summary>Indicates whether this feature is initially secret.</summary>
    public bool IsSecret { get; set; }

    /// <summary>Current visibility state of this feature.</summary>
    public VisibilityState VisibilityState { get; set; } = VisibilityState.Hidden;

    /// <summary>
    /// Discoverability difficulty class (0 = always visible, 1–20 = DC check required).
    /// A value of 0 on a non-secret feature means it is always visible.
    /// </summary>
    public int DiscoverabilityDC { get; set; }

    /// <summary>
    /// IDs of other Features that are automatically revealed when this feature
    /// transitions to <see cref="VisibilityState.Revealed"/>.
    /// </summary>
    public List<Guid> RevealPropagatesTo { get; set; } = [];

    /// <summary>The node this feature belongs to (nullable if attached to an edge).</summary>
    public Guid? NodeId { get; set; }

    /// <summary>The edge this feature belongs to (nullable if attached to a node).</summary>
    public Guid? EdgeId { get; set; }
}
