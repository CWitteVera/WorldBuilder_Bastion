namespace BastionBuilder.Domain.Enums;

/// <summary>
/// Specifies the kind of passage an Edge represents.
/// </summary>
public enum EdgeKind
{
    /// <summary>A standard door (hinged, sliding, etc.).</summary>
    Door = 0,

    /// <summary>A window or arrow slit.</summary>
    Window = 1,

    /// <summary>A staircase connecting different floors.</summary>
    Stair = 2,

    /// <summary>A floor hatch or ceiling hatch.</summary>
    Hatch = 3,

    /// <summary>An open archway or passage with no physical barrier.</summary>
    Archway = 4,
}
