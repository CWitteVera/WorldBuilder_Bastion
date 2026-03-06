namespace BastionBuilder.Domain.Entities;

/// <summary>
/// Represents an additive structural reinforcement applied to a wall segment.
/// Each reinforcement adds bonus AC and HP on top of the segment's base values.
/// </summary>
public class Reinforcement
{
    /// <summary>Unique identifier for this reinforcement.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>The wall segment this reinforcement is applied to.</summary>
    public Guid WallSegmentId { get; set; }

    /// <summary>Description of the reinforcement material or method.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Bonus armor class added additively to the segment's base AC.</summary>
    public int BonusAC { get; set; }

    /// <summary>Bonus hit points added additively to the segment's base HP.</summary>
    public int BonusHP { get; set; }
}
