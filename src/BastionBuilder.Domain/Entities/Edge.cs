using BastionBuilder.Domain.Enums;

namespace BastionBuilder.Domain.Entities;

/// <summary>
/// Represents a passage or connection between two nodes: a door, window, staircase, hatch, or archway.
/// Edges carry security metadata (lock, trap, alarm) and visibility state for secret passages.
/// </summary>
public class Edge
{
    /// <summary>Unique identifier for this edge.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>The type of passage this edge represents.</summary>
    public EdgeKind Kind { get; set; } = EdgeKind.Door;

    /// <summary>Display name of this edge (e.g., "Iron Gate", "Hidden Stair").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Full DM description of this edge.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Public-facing description visible once discovered.</summary>
    public string PublicDescription { get; set; } = string.Empty;

    /// <summary>The node on the "from" side of this edge.</summary>
    public Guid FromNodeId { get; set; }

    /// <summary>The node on the "to" side of this edge.</summary>
    public Guid ToNodeId { get; set; }

    // ── Discoverability ─────────────────────────────────────────────────────

    /// <summary>
    /// Discoverability difficulty class (0 = always visible, 1–20 = DC check required).
    /// </summary>
    public int DiscoverabilityDC { get; set; }

    /// <summary>Whether this edge (passage) is initially secret.</summary>
    public bool IsSecret { get; set; }

    /// <summary>Current visibility state of this edge.</summary>
    public VisibilityState VisibilityState { get; set; } = VisibilityState.Hidden;

    // ── Lock ────────────────────────────────────────────────────────────────

    /// <summary>Whether this edge has a lock.</summary>
    public bool HasLock { get; set; }

    /// <summary>
    /// Difficulty class to pick or bypass the lock (null if no lock).
    /// </summary>
    public int? LockDC { get; set; }

    /// <summary>Description of the lock type (e.g., "Masterwork padlock").</summary>
    public string? LockDescription { get; set; }

    // ── Trap ────────────────────────────────────────────────────────────────

    /// <summary>Whether this edge has a trap.</summary>
    public bool HasTrap { get; set; }

    /// <summary>
    /// Short kind description of the trap (e.g., "poison needle", "alarm bell").
    /// </summary>
    public string? TrapKind { get; set; }

    /// <summary>
    /// Difficulty class to detect this trap (null if no trap).
    /// </summary>
    public int? TrapDetectDC { get; set; }

    /// <summary>
    /// Difficulty class to disarm this trap (null if no trap).
    /// </summary>
    public int? TrapDisarmDC { get; set; }

    /// <summary>Damage or effect description of the trap.</summary>
    public string? TrapEffect { get; set; }

    // ── Alarm ───────────────────────────────────────────────────────────────

    /// <summary>Whether this edge has an alarm mechanism.</summary>
    public bool HasAlarm { get; set; }

    /// <summary>
    /// The perception DC at which the alarm triggers (null if no alarm).
    /// A value of 0 means the alarm always triggers when the edge is used.
    /// </summary>
    public int? AlarmThreshold { get; set; }

    /// <summary>Description of the alarm mechanism (e.g., "Bell wire", "Magic glyph").</summary>
    public string? AlarmDescription { get; set; }

    // ── Physical ────────────────────────────────────────────────────────────

    /// <summary>
    /// Height of the opening in feet.
    /// Stub for future height-scaling cover/movement calculations.
    /// </summary>
    public double HeightFeet { get; set; } = 7.0;

    /// <summary>Width of the opening in feet.</summary>
    public double WidthFeet { get; set; } = 5.0;

    /// <summary>Whether the edge is currently passable (e.g., doors can be open/closed).</summary>
    public bool IsPassable { get; set; } = true;

    // ── Features ────────────────────────────────────────────────────────────

    /// <summary>Additional features or points of interest on this edge.</summary>
    public List<Feature> Features { get; set; } = [];
}
