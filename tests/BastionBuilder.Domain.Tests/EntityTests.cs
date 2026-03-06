namespace BastionBuilder.Domain.Tests;

public class NodeTests
{
    [Fact]
    public void NewNode_HasDefaultValues()
    {
        var node = new Node();
        node.Id.Should().NotBe(Guid.Empty);
        node.Name.Should().BeEmpty();
        node.HeightFeet.Should().Be(10.0);
        node.DiscoverabilityDC.Should().Be(0);
        node.Features.Should().BeEmpty();
        node.Tags.Should().BeEmpty();
    }

    [Fact]
    public void Node_CanSetProperties()
    {
        var node = new Node
        {
            Name = "Guard Room",
            Description = "A room with guards",
            PublicDescription = "A locked door leads here",
            DiscoverabilityDC = 15,
            HeightFeet = 12.0,
            AreaSquareFeet = 400.0,
            FloorLevel = 1,
        };

        node.Name.Should().Be("Guard Room");
        node.DiscoverabilityDC.Should().Be(15);
        node.HeightFeet.Should().Be(12.0);
        node.FloorLevel.Should().Be(1);
    }

    [Fact]
    public void Node_CanAddFeatures()
    {
        var node = new Node { Name = "Treasure Room" };
        var feature = new Feature { Name = "Hidden chest", IsSecret = true };
        node.Features.Add(feature);

        node.Features.Should().HaveCount(1);
        node.Features[0].Name.Should().Be("Hidden chest");
    }
}

public class EdgeTests
{
    [Fact]
    public void NewEdge_HasDefaultValues()
    {
        var edge = new Edge();
        edge.Id.Should().NotBe(Guid.Empty);
        edge.Kind.Should().Be(EdgeKind.Door);
        edge.VisibilityState.Should().Be(VisibilityState.Hidden);
        edge.IsPassable.Should().BeTrue();
        edge.HeightFeet.Should().Be(7.0);
        edge.WidthFeet.Should().Be(5.0);
        edge.Features.Should().BeEmpty();
    }

    [Fact]
    public void Edge_LockFields_WorkCorrectly()
    {
        var edge = new Edge
        {
            HasLock = true,
            LockDC = 18,
            LockDescription = "Heavy iron padlock",
        };

        edge.HasLock.Should().BeTrue();
        edge.LockDC.Should().Be(18);
        edge.LockDescription.Should().Be("Heavy iron padlock");
    }

    [Fact]
    public void Edge_TrapFields_WorkCorrectly()
    {
        var edge = new Edge
        {
            HasTrap = true,
            TrapKind = "poison needle",
            TrapDetectDC = 14,
            TrapDisarmDC = 16,
            TrapEffect = "1d4 poison damage",
        };

        edge.HasTrap.Should().BeTrue();
        edge.TrapKind.Should().Be("poison needle");
        edge.TrapDetectDC.Should().Be(14);
        edge.TrapDisarmDC.Should().Be(16);
    }

    [Fact]
    public void Edge_AlarmFields_WorkCorrectly()
    {
        var edge = new Edge
        {
            HasAlarm = true,
            AlarmThreshold = 0,
            AlarmDescription = "Bell wire",
        };

        edge.HasAlarm.Should().BeTrue();
        edge.AlarmThreshold.Should().Be(0);
        edge.AlarmDescription.Should().Be("Bell wire");
    }

    [Fact]
    public void Edge_SecretPassage_HasCorrectDefaults()
    {
        var edge = new Edge { IsSecret = true };
        edge.IsSecret.Should().BeTrue();
        edge.VisibilityState.Should().Be(VisibilityState.Hidden);
    }
}

public class FeatureTests
{
    [Fact]
    public void NewFeature_HasDefaultValues()
    {
        var f = new Feature();
        f.Id.Should().NotBe(Guid.Empty);
        f.IsSecret.Should().BeFalse();
        f.VisibilityState.Should().Be(VisibilityState.Hidden);
        f.DiscoverabilityDC.Should().Be(0);
        f.RevealPropagatesTo.Should().BeEmpty();
    }

    [Fact]
    public void Feature_RevealPropagation_CanSetTargetIds()
    {
        var target = new Feature { Name = "Second secret" };
        var source = new Feature
        {
            Name = "First secret",
            IsSecret = true,
            RevealPropagatesTo = [target.Id],
        };

        source.RevealPropagatesTo.Should().ContainSingle()
            .Which.Should().Be(target.Id);
    }
}

public class WallSegmentTests
{
    [Fact]
    public void NewWallSegment_HasDefaultValues()
    {
        var seg = new WallSegment();
        seg.Id.Should().NotBe(Guid.Empty);
        seg.BaseAC.Should().Be(15);
        seg.BaseHP.Should().Be(27);
        seg.HeightFeet.Should().Be(10.0);
        seg.ThicknessInches.Should().Be(12.0);
        seg.Reinforcements.Should().BeEmpty();
        seg.OpeningSplits.Should().BeEmpty();
    }

    [Fact]
    public void WallSegment_CanAddReinforcements()
    {
        var seg = new WallSegment { BaseAC = 15, BaseHP = 27 };
        seg.Reinforcements.Add(new Reinforcement { BonusAC = 2, BonusHP = 10 });
        seg.Reinforcements.Add(new Reinforcement { BonusAC = 1, BonusHP = 5 });

        seg.Reinforcements.Should().HaveCount(2);
    }

    [Fact]
    public void WallSegment_SharedWalls_CanHaveMultipleNodes()
    {
        var nodeA = Guid.NewGuid();
        var nodeB = Guid.NewGuid();
        var nodeC = Guid.NewGuid();

        var seg = new WallSegment
        {
            FromNodeId = nodeA,
            ToNodeId = nodeB,
            SharedWithNodeIds = [nodeC],
        };

        seg.SharedWithNodeIds.Should().ContainSingle()
            .Which.Should().Be(nodeC);
    }
}

public class OpeningSplitTests
{
    [Fact]
    public void OpeningSplit_HasCorrectDefaults()
    {
        var split = new OpeningSplit();
        split.Id.Should().NotBe(Guid.Empty);
        split.SplitPercentage.Should().Be(0);
        split.EdgeId.Should().BeNull();
    }

    [Fact]
    public void OpeningSplit_CanSetPercentage()
    {
        var split = new OpeningSplit { SplitPercentage = 0.25 };
        split.SplitPercentage.Should().Be(0.25);
    }
}

public class BastionAggregateTests
{
    [Fact]
    public void NewBastion_HasDefaultValues()
    {
        var b = new Bastion();
        b.Id.Should().NotBe(Guid.Empty);
        b.Nodes.Should().BeEmpty();
        b.Edges.Should().BeEmpty();
        b.WallGroups.Should().BeEmpty();
        b.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Bastion_CanAddNodes()
    {
        var bastion = new Bastion { Name = "Fort Danger" };
        bastion.Nodes.Add(new Node { Name = "Entrance Hall" });
        bastion.Nodes.Add(new Node { Name = "Guard Post" });

        bastion.Nodes.Should().HaveCount(2);
    }
}
