using BastionBuilder.Domain.Enums;

namespace BastionBuilder.Export.Dtos;

// ── Public DTOs (player-visible) ────────────────────────────────────────────

/// <summary>Top-level public export of a bastion (secrets hidden).</summary>
public record PublicBastionDto(
    Guid Id,
    string Name,
    string Description,
    IReadOnlyList<PublicNodeDto> Nodes,
    IReadOnlyList<PublicEdgeDto> Edges
);

/// <summary>Public view of a room node.</summary>
public record PublicNodeDto(
    Guid Id,
    string Name,
    string PublicDescription,
    int FloorLevel,
    IReadOnlyList<PublicFeatureDto> VisibleFeatures
);

/// <summary>Public view of a passage edge.</summary>
public record PublicEdgeDto(
    Guid Id,
    string Name,
    string PublicDescription,
    EdgeKind Kind,
    Guid FromNodeId,
    Guid ToNodeId,
    bool IsPassable,
    bool HasLock,
    bool HasTrap,
    bool HasAlarm,
    IReadOnlyList<PublicFeatureDto> VisibleFeatures
);

/// <summary>Public view of a discoverable feature.</summary>
public record PublicFeatureDto(
    Guid Id,
    string Name,
    string PublicDescription,
    VisibilityState VisibilityState
);

// ── DM DTOs (full view, all secrets included) ────────────────────────────────

/// <summary>Full DM export of a bastion.</summary>
public record DmBastionDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAtUtc,
    DateTime ModifiedAtUtc,
    IReadOnlyList<DmNodeDto> Nodes,
    IReadOnlyList<DmEdgeDto> Edges,
    IReadOnlyList<DmWallGroupDto> WallGroups
);

/// <summary>Full DM view of a room node.</summary>
public record DmNodeDto(
    Guid Id,
    string Name,
    string Description,
    string PublicDescription,
    int DiscoverabilityDC,
    double HeightFeet,
    double AreaSquareFeet,
    double PositionX,
    double PositionY,
    int FloorLevel,
    IReadOnlyList<string> Tags,
    IReadOnlyList<DmFeatureDto> Features
);

/// <summary>Full DM view of a passage edge.</summary>
public record DmEdgeDto(
    Guid Id,
    string Name,
    string Description,
    string PublicDescription,
    EdgeKind Kind,
    Guid FromNodeId,
    Guid ToNodeId,
    int DiscoverabilityDC,
    bool IsSecret,
    VisibilityState VisibilityState,
    bool HasLock,
    int? LockDC,
    string? LockDescription,
    bool HasTrap,
    string? TrapKind,
    int? TrapDetectDC,
    int? TrapDisarmDC,
    string? TrapEffect,
    bool HasAlarm,
    int? AlarmThreshold,
    string? AlarmDescription,
    double HeightFeet,
    double WidthFeet,
    bool IsPassable,
    IReadOnlyList<DmFeatureDto> Features
);

/// <summary>Full DM view of a feature.</summary>
public record DmFeatureDto(
    Guid Id,
    string Name,
    string Description,
    string PublicDescription,
    bool IsSecret,
    VisibilityState VisibilityState,
    int DiscoverabilityDC,
    IReadOnlyList<Guid> RevealPropagatesTo
);

/// <summary>Full DM view of a wall group.</summary>
public record DmWallGroupDto(
    Guid Id,
    string Name,
    string Description,
    int FloorLevel,
    IReadOnlyList<DmWallSegmentDto> Segments
);

/// <summary>Full DM view of a wall segment.</summary>
public record DmWallSegmentDto(
    Guid Id,
    Guid FromNodeId,
    Guid? ToNodeId,
    IReadOnlyList<Guid> SharedWithNodeIds,
    string Material,
    double ThicknessInches,
    int BaseAC,
    int BaseHP,
    int EffectiveAC,
    int EffectiveHP,
    double HeightFeet,
    double LengthFeet,
    double HeightScaleFactor,
    double SolidFraction,
    IReadOnlyList<DmReinforcementDto> Reinforcements,
    IReadOnlyList<DmOpeningSplitDto> OpeningSplits
);

/// <summary>DM view of a wall reinforcement.</summary>
public record DmReinforcementDto(
    Guid Id,
    string Description,
    int BonusAC,
    int BonusHP
);

/// <summary>DM view of an opening split.</summary>
public record DmOpeningSplitDto(
    Guid Id,
    string Description,
    double SplitPercentage,
    Guid? EdgeId
);
