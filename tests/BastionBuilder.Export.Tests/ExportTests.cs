using System.Text.Json;

namespace BastionBuilder.Export.Tests;

public class JsonExportServiceTests
{
    private static Bastion BuildSampleBastion()
    {
        var nodeA = new Node { Name = "Entrance", PublicDescription = "A grand entrance", FloorLevel = 0 };
        var nodeB = new Node
        {
            Name = "Secret Chamber",
            PublicDescription = "Appears to be a dead end",
            Description = "Hidden treasure room",
            DiscoverabilityDC = 18,
        };
        var secretFeature = new Feature
        {
            Name = "Hidden Lever",
            Description = "Opens the east wall",
            PublicDescription = "A wall sconce",
            IsSecret = true,
            VisibilityState = VisibilityState.Hidden,
            DiscoverabilityDC = 16,
        };
        var visibleFeature = new Feature
        {
            Name = "Fireplace",
            Description = "Decorative fireplace",
            PublicDescription = "A roaring fireplace",
            IsSecret = false,
            VisibilityState = VisibilityState.Revealed,
        };
        nodeA.Features.Add(secretFeature);
        nodeA.Features.Add(visibleFeature);

        var secretEdge = new Edge
        {
            Name = "Hidden Door",
            Kind = EdgeKind.Door,
            FromNodeId = nodeA.Id,
            ToNodeId = nodeB.Id,
            IsSecret = true,
            VisibilityState = VisibilityState.Hidden,
            DiscoverabilityDC = 20,
            HasLock = true,
            LockDC = 18,
            HasTrap = true,
            TrapKind = "poison needle",
            TrapDetectDC = 14,
            HasAlarm = true,
            AlarmThreshold = 0,
        };
        var normalEdge = new Edge
        {
            Name = "Main Gate",
            Kind = EdgeKind.Door,
            FromNodeId = nodeA.Id,
            ToNodeId = nodeB.Id,
            IsSecret = false,
            VisibilityState = VisibilityState.Revealed,
            IsPassable = true,
        };

        var reinforcement = new Reinforcement { BonusAC = 3, BonusHP = 15, Description = "Iron banding" };
        var opening = new OpeningSplit { SplitPercentage = 0.1, Description = "Arrow slit" };
        var segment = new WallSegment
        {
            FromNodeId = nodeA.Id,
            ToNodeId = nodeB.Id,
            BaseAC = 15,
            BaseHP = 27,
            Material = "Stone",
        };
        segment.Reinforcements.Add(reinforcement);
        segment.OpeningSplits.Add(opening);
        var wallGroup = new WallGroup { Name = "Outer Wall" };
        wallGroup.Segments.Add(segment);

        var bastion = new Bastion { Name = "Fort Test", Description = "A test bastion" };
        bastion.Nodes.Add(nodeA);
        bastion.Nodes.Add(nodeB);
        bastion.Edges.Add(secretEdge);
        bastion.Edges.Add(normalEdge);
        bastion.WallGroups.Add(wallGroup);

        return bastion;
    }

    // ── Public export ────────────────────────────────────────────────────────

    [Fact]
    public void ExportPublicJson_ReturnsValidJson()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportPublicJson(bastion);

        json.Should().NotBeNullOrEmpty();
        Action parse = () => JsonDocument.Parse(json);
        parse.Should().NotThrow();
    }

    [Fact]
    public void ExportPublicJson_ExcludesHiddenSecretEdge()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportPublicJson(bastion);
        var doc = JsonDocument.Parse(json);

        JsonElement edges = doc.RootElement.GetProperty("edges");
        // Only the non-secret edge should appear
        edges.GetArrayLength().Should().Be(1);
        edges[0].GetProperty("name").GetString().Should().Be("Main Gate");
    }

    [Fact]
    public void ExportPublicJson_ExcludesHiddenSecretFeatures()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportPublicJson(bastion);
        var doc = JsonDocument.Parse(json);

        JsonElement nodes = doc.RootElement.GetProperty("nodes");
        // Entrance node features: only the visible "Fireplace" should appear
        JsonElement entrance = nodes.EnumerateArray()
            .First(n => n.GetProperty("name").GetString() == "Entrance");

        JsonElement features = entrance.GetProperty("visibleFeatures");
        features.GetArrayLength().Should().Be(1);
        features[0].GetProperty("name").GetString().Should().Be("Fireplace");
    }

    [Fact]
    public void ExportPublicJson_IncludesDiscoveredSecretEdge()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();
        // Mark the secret edge as discovered
        bastion.Edges[0].VisibilityState = VisibilityState.Discovered;

        string json = svc.ExportPublicJson(bastion);
        var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("edges").GetArrayLength().Should().Be(2);
    }

    // ── DM export ────────────────────────────────────────────────────────────

    [Fact]
    public void ExportDmJson_ReturnsValidJson()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportDmJson(bastion);

        json.Should().NotBeNullOrEmpty();
        Action parse = () => JsonDocument.Parse(json);
        parse.Should().NotThrow();
    }

    [Fact]
    public void ExportDmJson_IncludesAllEdgesIncludingSecret()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportDmJson(bastion);
        var doc = JsonDocument.Parse(json);

        doc.RootElement.GetProperty("edges").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public void ExportDmJson_IncludesAllFeatures()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportDmJson(bastion);
        var doc = JsonDocument.Parse(json);

        JsonElement entrance = doc.RootElement.GetProperty("nodes").EnumerateArray()
            .First(n => n.GetProperty("name").GetString() == "Entrance");

        // Both features should be present in DM view
        entrance.GetProperty("features").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public void ExportDmJson_WallGroupsContainEffectiveAcHp()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportDmJson(bastion);
        var doc = JsonDocument.Parse(json);

        JsonElement segment = doc.RootElement
            .GetProperty("wallGroups")[0]
            .GetProperty("segments")[0];

        segment.GetProperty("effectiveAC").GetInt32().Should().Be(18); // 15 + 3
        segment.GetProperty("effectiveHP").GetInt32().Should().Be(42); // 27 + 15
    }

    [Fact]
    public void ExportDmJson_WallSegmentContainsHeightScaleAndSolidFraction()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportDmJson(bastion);
        var doc = JsonDocument.Parse(json);

        JsonElement segment = doc.RootElement
            .GetProperty("wallGroups")[0]
            .GetProperty("segments")[0];

        segment.GetProperty("heightScaleFactor").GetDouble().Should().BeApproximately(1.0, 0.01);
        segment.GetProperty("solidFraction").GetDouble().Should().BeApproximately(0.9, 0.01);
    }

    [Fact]
    public void ExportDmJson_EdgeContainsLockTrapAlarmFields()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();

        string json = svc.ExportDmJson(bastion);
        var doc = JsonDocument.Parse(json);

        JsonElement secretEdge = doc.RootElement.GetProperty("edges").EnumerateArray()
            .First(e => e.GetProperty("name").GetString() == "Hidden Door");

        secretEdge.GetProperty("hasLock").GetBoolean().Should().BeTrue();
        secretEdge.GetProperty("lockDC").GetInt32().Should().Be(18);
        secretEdge.GetProperty("hasTrap").GetBoolean().Should().BeTrue();
        secretEdge.GetProperty("trapKind").GetString().Should().Be("poison needle");
        secretEdge.GetProperty("hasAlarm").GetBoolean().Should().BeTrue();
        secretEdge.GetProperty("alarmThreshold").GetInt32().Should().Be(0);
    }

    [Fact]
    public void ExportPublicJson_NullBastion_ThrowsArgumentNullException()
    {
        var svc = new JsonExportService();
        Action act = () => svc.ExportPublicJson(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExportDmJson_NullBastion_ThrowsArgumentNullException()
    {
        var svc = new JsonExportService();
        Action act = () => svc.ExportDmJson(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task ExportPublicToFileAsync_WritesFile()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();
        string path = Path.Combine(Path.GetTempPath(), $"bastion_public_{Guid.NewGuid():N}.json");

        try
        {
            await svc.ExportPublicToFileAsync(bastion, path);

            File.Exists(path).Should().BeTrue();
            string content = await File.ReadAllTextAsync(path);
            content.Should().Contain("Fort Test");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public async Task ExportDmToFileAsync_WritesFile()
    {
        var svc = new JsonExportService();
        var bastion = BuildSampleBastion();
        string path = Path.Combine(Path.GetTempPath(), $"bastion_dm_{Guid.NewGuid():N}.json");

        try
        {
            await svc.ExportDmToFileAsync(bastion, path);

            File.Exists(path).Should().BeTrue();
            string content = await File.ReadAllTextAsync(path);
            content.Should().Contain("Hidden Door");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
