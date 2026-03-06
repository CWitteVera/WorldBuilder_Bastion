using System.Text.Json;
using System.Text.Json.Serialization;
using BastionBuilder.Application.Interfaces;
using BastionBuilder.Domain.Entities;
using BastionBuilder.Domain.Enums;
using BastionBuilder.Export.Dtos;
using BastionBuilder.Rules;

namespace BastionBuilder.Export;

/// <summary>
/// JSON implementation of <see cref="IExportService"/>.
/// Produces a "public" export (secrets redacted) and a "DM" export (everything visible).
/// </summary>
public class JsonExportService : IExportService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    // ── Public export ────────────────────────────────────────────────────────

    /// <inheritdoc />
    public string ExportPublicJson(Bastion bastion)
    {
        ArgumentNullException.ThrowIfNull(bastion);
        PublicBastionDto dto = MapPublic(bastion);
        return JsonSerializer.Serialize(dto, SerializerOptions);
    }

    /// <inheritdoc />
    public async Task ExportPublicToFileAsync(
        Bastion bastion,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bastion);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string json = ExportPublicJson(bastion);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    // ── DM export ────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public string ExportDmJson(Bastion bastion)
    {
        ArgumentNullException.ThrowIfNull(bastion);
        DmBastionDto dto = MapDm(bastion);
        return JsonSerializer.Serialize(dto, SerializerOptions);
    }

    /// <inheritdoc />
    public async Task ExportDmToFileAsync(
        Bastion bastion,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bastion);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string json = ExportDmJson(bastion);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    // ── Mapping helpers ──────────────────────────────────────────────────────

    private static PublicBastionDto MapPublic(Bastion bastion)
    {
        // Only include nodes that are not completely hidden
        List<PublicNodeDto> nodes = bastion.Nodes
            .Select(MapPublicNode)
            .ToList();

        // Only include edges that are not secret/hidden
        List<PublicEdgeDto> edges = bastion.Edges
            .Where(e => !e.IsSecret || e.VisibilityState != VisibilityState.Hidden)
            .Select(MapPublicEdge)
            .ToList();

        return new PublicBastionDto(
            bastion.Id,
            bastion.Name,
            bastion.Description,
            nodes,
            edges);
    }

    private static PublicNodeDto MapPublicNode(Node node)
    {
        List<PublicFeatureDto> visibleFeatures = node.Features
            .Where(f => !f.IsSecret || f.VisibilityState != VisibilityState.Hidden)
            .Select(MapPublicFeature)
            .ToList();

        return new PublicNodeDto(
            node.Id,
            node.Name,
            node.PublicDescription,
            node.FloorLevel,
            visibleFeatures);
    }

    private static PublicEdgeDto MapPublicEdge(Edge edge)
    {
        List<PublicFeatureDto> visibleFeatures = edge.Features
            .Where(f => !f.IsSecret || f.VisibilityState != VisibilityState.Hidden)
            .Select(MapPublicFeature)
            .ToList();

        return new PublicEdgeDto(
            edge.Id,
            edge.Name,
            edge.PublicDescription,
            edge.Kind,
            edge.FromNodeId,
            edge.ToNodeId,
            edge.IsPassable,
            edge.HasLock,
            edge.HasTrap,
            edge.HasAlarm,
            visibleFeatures);
    }

    private static PublicFeatureDto MapPublicFeature(Feature feature)
        => new(feature.Id, feature.Name, feature.PublicDescription, feature.VisibilityState);

    private static DmBastionDto MapDm(Bastion bastion)
        => new(
            bastion.Id,
            bastion.Name,
            bastion.Description,
            bastion.CreatedAtUtc,
            bastion.ModifiedAtUtc,
            bastion.Nodes.Select(MapDmNode).ToList(),
            bastion.Edges.Select(MapDmEdge).ToList(),
            bastion.WallGroups.Select(MapDmWallGroup).ToList());

    private static DmNodeDto MapDmNode(Node node)
        => new(
            node.Id,
            node.Name,
            node.Description,
            node.PublicDescription,
            node.DiscoverabilityDC,
            node.HeightFeet,
            node.AreaSquareFeet,
            node.PositionX,
            node.PositionY,
            node.FloorLevel,
            node.Tags,
            node.Features.Select(MapDmFeature).ToList());

    private static DmEdgeDto MapDmEdge(Edge edge)
        => new(
            edge.Id,
            edge.Name,
            edge.Description,
            edge.PublicDescription,
            edge.Kind,
            edge.FromNodeId,
            edge.ToNodeId,
            edge.DiscoverabilityDC,
            edge.IsSecret,
            edge.VisibilityState,
            edge.HasLock,
            edge.LockDC,
            edge.LockDescription,
            edge.HasTrap,
            edge.TrapKind,
            edge.TrapDetectDC,
            edge.TrapDisarmDC,
            edge.TrapEffect,
            edge.HasAlarm,
            edge.AlarmThreshold,
            edge.AlarmDescription,
            edge.HeightFeet,
            edge.WidthFeet,
            edge.IsPassable,
            edge.Features.Select(MapDmFeature).ToList());

    private static DmFeatureDto MapDmFeature(Feature f)
        => new(f.Id, f.Name, f.Description, f.PublicDescription,
               f.IsSecret, f.VisibilityState, f.DiscoverabilityDC, f.RevealPropagatesTo);

    private static DmWallGroupDto MapDmWallGroup(WallGroup wg)
        => new(wg.Id, wg.Name, wg.Description, wg.FloorLevel,
               wg.Segments.Select(MapDmWallSegment).ToList());

    private static DmWallSegmentDto MapDmWallSegment(WallSegment seg)
        => new(
            seg.Id,
            seg.FromNodeId,
            seg.ToNodeId,
            seg.SharedWithNodeIds,
            seg.Material,
            seg.ThicknessInches,
            seg.BaseAC,
            seg.BaseHP,
            WallSegmentRules.EffectiveAC(seg),
            WallSegmentRules.EffectiveHP(seg),
            seg.HeightFeet,
            seg.LengthFeet,
            WallSegmentRules.HeightScaleFactor(seg),
            WallSegmentRules.SolidFraction(seg),
            seg.Reinforcements.Select(r => new DmReinforcementDto(r.Id, r.Description, r.BonusAC, r.BonusHP)).ToList(),
            seg.OpeningSplits.Select(o => new DmOpeningSplitDto(o.Id, o.Description, o.SplitPercentage, o.EdgeId)).ToList());
}
