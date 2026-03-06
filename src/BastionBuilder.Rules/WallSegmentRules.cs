using BastionBuilder.Domain.Entities;

namespace BastionBuilder.Rules;

/// <summary>
/// Calculates effective statistics for a <see cref="WallSegment"/> after applying
/// additive reinforcements and a height-scaling stub.
/// </summary>
public static class WallSegmentRules
{
    /// <summary>
    /// Computes the effective armor class of a wall segment.
    /// Each <see cref="Reinforcement"/> adds its <c>BonusAC</c> on top of the base.
    /// </summary>
    /// <param name="segment">The wall segment to evaluate.</param>
    /// <returns>Effective armor class (base + sum of all reinforcement bonuses).</returns>
    public static int EffectiveAC(WallSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);
        return segment.BaseAC + segment.Reinforcements.Sum(r => r.BonusAC);
    }

    /// <summary>
    /// Computes the effective hit points of a wall segment.
    /// Each <see cref="Reinforcement"/> adds its <c>BonusHP</c> on top of the base.
    /// </summary>
    /// <param name="segment">The wall segment to evaluate.</param>
    /// <returns>Effective hit points (base + sum of all reinforcement bonuses).</returns>
    public static int EffectiveHP(WallSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);
        return segment.BaseHP + segment.Reinforcements.Sum(r => r.BonusHP);
    }

    /// <summary>
    /// Height-scaling stub: returns a multiplier based on wall height compared to a
    /// reference height of 10 ft.  Currently linear; intended as an extension point for
    /// more sophisticated damage/cover formulas.
    /// </summary>
    /// <param name="segment">The wall segment to evaluate.</param>
    /// <returns>Height scale factor (HeightFeet / 10.0), minimum 0.1.</returns>
    public static double HeightScaleFactor(WallSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);
        const double referenceHeight = 10.0;
        return Math.Max(0.1, segment.HeightFeet / referenceHeight);
    }

    /// <summary>
    /// Returns the total solid fraction of a wall segment after subtracting opening splits.
    /// A value of 1.0 means no openings; 0.0 would mean entirely open (degenerate).
    /// </summary>
    /// <param name="segment">The wall segment to evaluate.</param>
    /// <returns>Solid fraction clamped to [0.0, 1.0].</returns>
    public static double SolidFraction(WallSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);
        double totalOpen = segment.OpeningSplits.Sum(o => o.SplitPercentage);
        return Math.Clamp(1.0 - totalOpen, 0.0, 1.0);
    }
}
