namespace BastionBuilder.Domain.Enums;

/// <summary>
/// Represents the visibility state of a secret element in the bastion.
/// </summary>
public enum VisibilityState
{
    /// <summary>The element is hidden and has not yet been detected.</summary>
    Hidden = 0,

    /// <summary>The element has been found but not fully investigated.</summary>
    Discovered = 1,

    /// <summary>The element is fully revealed and known to the players.</summary>
    Revealed = 2,
}
