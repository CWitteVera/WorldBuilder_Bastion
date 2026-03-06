using BastionBuilder.Domain.Entities;
using BastionBuilder.Domain.Enums;

namespace BastionBuilder.Rules;

/// <summary>
/// Rules governing secret discovery and reveal propagation for
/// <see cref="Feature"/> and <see cref="Edge"/> entities.
/// </summary>
public static class DiscoverabilityRules
{
    /// <summary>
    /// Determines whether a given perception roll beats (or ties) the discoverability DC.
    /// A DC of 0 always succeeds (the element is not secret).
    /// </summary>
    /// <param name="discoverabilityDC">The target DC (0–20).</param>
    /// <param name="roll">The player's perception roll total (includes modifiers).</param>
    /// <returns><c>true</c> if the roll meets or exceeds the DC.</returns>
    public static bool Discovers(int discoverabilityDC, int roll)
    {
        if (discoverabilityDC <= 0) return true;
        return roll >= discoverabilityDC;
    }

    /// <summary>
    /// Attempts to discover a <see cref="Feature"/> using the given perception roll.
    /// If the roll meets the DC the feature transitions from
    /// <see cref="VisibilityState.Hidden"/> to <see cref="VisibilityState.Discovered"/>.
    /// </summary>
    /// <param name="feature">The feature to attempt discovery on.</param>
    /// <param name="roll">The perception roll total.</param>
    /// <returns><c>true</c> if the feature's state changed.</returns>
    public static bool TryDiscover(Feature feature, int roll)
    {
        ArgumentNullException.ThrowIfNull(feature);
        if (feature.VisibilityState != VisibilityState.Hidden) return false;
        if (!Discovers(feature.DiscoverabilityDC, roll)) return false;

        feature.VisibilityState = VisibilityState.Discovered;
        return true;
    }

    /// <summary>
    /// Reveals a feature and propagates the reveal to any linked features found in
    /// <paramref name="allFeatures"/>.
    /// </summary>
    /// <param name="feature">The feature to reveal.</param>
    /// <param name="allFeatures">
    /// The complete collection of features to search when propagating reveals.
    /// </param>
    /// <returns>All features whose state changed (including <paramref name="feature"/>).</returns>
    public static IReadOnlyList<Feature> Reveal(Feature feature, IEnumerable<Feature> allFeatures)
    {
        ArgumentNullException.ThrowIfNull(feature);
        ArgumentNullException.ThrowIfNull(allFeatures);

        var changed = new List<Feature>();
        RevealInternal(feature, allFeatures.ToList(), changed, new HashSet<Guid>());
        return changed;
    }

    private static void RevealInternal(
        Feature feature,
        List<Feature> allFeatures,
        List<Feature> changed,
        HashSet<Guid> visited)
    {
        if (!visited.Add(feature.Id)) return; // prevent cycles

        if (feature.VisibilityState != VisibilityState.Revealed)
        {
            feature.VisibilityState = VisibilityState.Revealed;
            changed.Add(feature);
        }

        foreach (Guid propagateId in feature.RevealPropagatesTo)
        {
            Feature? target = allFeatures.Find(f => f.Id == propagateId);
            if (target is not null)
            {
                RevealInternal(target, allFeatures, changed, visited);
            }
        }
    }

    /// <summary>
    /// Attempts to discover an <see cref="Edge"/> (secret passage) with a perception roll.
    /// </summary>
    /// <param name="edge">The edge to attempt discovery on.</param>
    /// <param name="roll">The perception roll total.</param>
    /// <returns><c>true</c> if the edge's state changed.</returns>
    public static bool TryDiscoverEdge(Edge edge, int roll)
    {
        ArgumentNullException.ThrowIfNull(edge);
        if (edge.VisibilityState != VisibilityState.Hidden) return false;
        if (!Discovers(edge.DiscoverabilityDC, roll)) return false;

        edge.VisibilityState = VisibilityState.Discovered;
        return true;
    }

    /// <summary>
    /// Reveals an <see cref="Edge"/> (secret passage) fully.
    /// </summary>
    /// <param name="edge">The edge to reveal.</param>
    /// <returns><c>true</c> if the edge's state changed.</returns>
    public static bool RevealEdge(Edge edge)
    {
        ArgumentNullException.ThrowIfNull(edge);
        if (edge.VisibilityState == VisibilityState.Revealed) return false;

        edge.VisibilityState = VisibilityState.Revealed;
        return true;
    }
}
