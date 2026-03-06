namespace BastionBuilder.Rules.Tests;

public class WallSegmentRulesTests
{
    [Fact]
    public void EffectiveAC_NoReinforcements_ReturnsBaseAC()
    {
        var seg = new WallSegment { BaseAC = 15 };
        WallSegmentRules.EffectiveAC(seg).Should().Be(15);
    }

    [Fact]
    public void EffectiveAC_WithReinforcements_AddsAdditively()
    {
        var seg = new WallSegment { BaseAC = 15 };
        seg.Reinforcements.Add(new Reinforcement { BonusAC = 2 });
        seg.Reinforcements.Add(new Reinforcement { BonusAC = 3 });

        WallSegmentRules.EffectiveAC(seg).Should().Be(20); // 15 + 2 + 3
    }

    [Fact]
    public void EffectiveHP_NoReinforcements_ReturnsBaseHP()
    {
        var seg = new WallSegment { BaseHP = 27 };
        WallSegmentRules.EffectiveHP(seg).Should().Be(27);
    }

    [Fact]
    public void EffectiveHP_WithReinforcements_AddsAdditively()
    {
        var seg = new WallSegment { BaseHP = 27 };
        seg.Reinforcements.Add(new Reinforcement { BonusHP = 10 });
        seg.Reinforcements.Add(new Reinforcement { BonusHP = 5 });

        WallSegmentRules.EffectiveHP(seg).Should().Be(42); // 27 + 10 + 5
    }

    [Fact]
    public void EffectiveAC_NullSegment_ThrowsArgumentNullException()
    {
        Action act = () => WallSegmentRules.EffectiveAC(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void HeightScaleFactor_AtReferenceHeight_ReturnsOne()
    {
        var seg = new WallSegment { HeightFeet = 10.0 };
        WallSegmentRules.HeightScaleFactor(seg).Should().BeApproximately(1.0, 0.001);
    }

    [Fact]
    public void HeightScaleFactor_DoubleHeight_ReturnsTwo()
    {
        var seg = new WallSegment { HeightFeet = 20.0 };
        WallSegmentRules.HeightScaleFactor(seg).Should().BeApproximately(2.0, 0.001);
    }

    [Fact]
    public void HeightScaleFactor_ZeroHeight_ReturnsMinimum()
    {
        var seg = new WallSegment { HeightFeet = 0.0 };
        WallSegmentRules.HeightScaleFactor(seg).Should().BeGreaterThan(0);
        WallSegmentRules.HeightScaleFactor(seg).Should().Be(0.1);
    }

    [Fact]
    public void SolidFraction_NoOpenings_ReturnsOne()
    {
        var seg = new WallSegment();
        WallSegmentRules.SolidFraction(seg).Should().Be(1.0);
    }

    [Fact]
    public void SolidFraction_SingleOpening_SubtractsCorrectly()
    {
        var seg = new WallSegment();
        seg.OpeningSplits.Add(new OpeningSplit { SplitPercentage = 0.25 });

        WallSegmentRules.SolidFraction(seg).Should().BeApproximately(0.75, 0.001);
    }

    [Fact]
    public void SolidFraction_MultipleOpenings_ClampsAtZero()
    {
        var seg = new WallSegment();
        seg.OpeningSplits.Add(new OpeningSplit { SplitPercentage = 0.6 });
        seg.OpeningSplits.Add(new OpeningSplit { SplitPercentage = 0.6 });

        WallSegmentRules.SolidFraction(seg).Should().Be(0.0); // clamped
    }
}

public class DiscoverabilityRulesTests
{
    [Theory]
    [InlineData(0, 1, true)]    // DC 0 always succeeds
    [InlineData(0, 0, true)]    // DC 0 always succeeds
    [InlineData(15, 15, true)]  // tie succeeds
    [InlineData(15, 16, true)]  // beat succeeds
    [InlineData(15, 14, false)] // miss fails
    [InlineData(20, 19, false)] // miss fails
    [InlineData(20, 20, true)]  // tie succeeds
    public void Discovers_ReturnsExpected(int dc, int roll, bool expected)
    {
        DiscoverabilityRules.Discovers(dc, roll).Should().Be(expected);
    }

    [Fact]
    public void TryDiscover_Hidden_RollBeats_SetsDiscovered()
    {
        var feature = new Feature { IsSecret = true, DiscoverabilityDC = 12, VisibilityState = VisibilityState.Hidden };
        bool changed = DiscoverabilityRules.TryDiscover(feature, 15);

        changed.Should().BeTrue();
        feature.VisibilityState.Should().Be(VisibilityState.Discovered);
    }

    [Fact]
    public void TryDiscover_Hidden_RollMisses_RemainsHidden()
    {
        var feature = new Feature { IsSecret = true, DiscoverabilityDC = 18, VisibilityState = VisibilityState.Hidden };
        bool changed = DiscoverabilityRules.TryDiscover(feature, 10);

        changed.Should().BeFalse();
        feature.VisibilityState.Should().Be(VisibilityState.Hidden);
    }

    [Fact]
    public void TryDiscover_AlreadyDiscovered_ReturnsFalse()
    {
        var feature = new Feature { DiscoverabilityDC = 5, VisibilityState = VisibilityState.Discovered };
        bool changed = DiscoverabilityRules.TryDiscover(feature, 20);

        changed.Should().BeFalse();
    }

    [Fact]
    public void Reveal_SetsRevealedAndPropagates()
    {
        var child = new Feature { Name = "child", VisibilityState = VisibilityState.Hidden };
        var parent = new Feature
        {
            Name = "parent",
            VisibilityState = VisibilityState.Hidden,
            RevealPropagatesTo = [child.Id],
        };
        var allFeatures = new List<Feature> { parent, child };

        IReadOnlyList<Feature> changed = DiscoverabilityRules.Reveal(parent, allFeatures);

        changed.Should().HaveCount(2);
        parent.VisibilityState.Should().Be(VisibilityState.Revealed);
        child.VisibilityState.Should().Be(VisibilityState.Revealed);
    }

    [Fact]
    public void Reveal_DoesNotCycle_WithCircularReferences()
    {
        var a = new Feature { Name = "A", VisibilityState = VisibilityState.Hidden };
        var b = new Feature { Name = "B", VisibilityState = VisibilityState.Hidden };
        a.RevealPropagatesTo.Add(b.Id);
        b.RevealPropagatesTo.Add(a.Id); // circular

        var allFeatures = new List<Feature> { a, b };

        // Should not throw or loop infinitely
        Action act = () => DiscoverabilityRules.Reveal(a, allFeatures);
        act.Should().NotThrow();

        a.VisibilityState.Should().Be(VisibilityState.Revealed);
        b.VisibilityState.Should().Be(VisibilityState.Revealed);
    }

    [Fact]
    public void TryDiscoverEdge_Hidden_RollBeats_SetsDiscovered()
    {
        var edge = new Edge { IsSecret = true, DiscoverabilityDC = 12, VisibilityState = VisibilityState.Hidden };
        bool changed = DiscoverabilityRules.TryDiscoverEdge(edge, 14);

        changed.Should().BeTrue();
        edge.VisibilityState.Should().Be(VisibilityState.Discovered);
    }

    [Fact]
    public void RevealEdge_Hidden_SetsRevealed()
    {
        var edge = new Edge { IsSecret = true, VisibilityState = VisibilityState.Hidden };
        bool changed = DiscoverabilityRules.RevealEdge(edge);

        changed.Should().BeTrue();
        edge.VisibilityState.Should().Be(VisibilityState.Revealed);
    }

    [Fact]
    public void RevealEdge_AlreadyRevealed_ReturnsFalse()
    {
        var edge = new Edge { VisibilityState = VisibilityState.Revealed };
        bool changed = DiscoverabilityRules.RevealEdge(edge);

        changed.Should().BeFalse();
    }
}
