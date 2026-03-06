namespace BastionBuilder.Application.Tests;

// ── Fake repository ──────────────────────────────────────────────────────────

file sealed class FakeBastionRepository : IBastionRepository
{
    private readonly Dictionary<Guid, Bastion> _store = [];

    public Task<IReadOnlyList<Bastion>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Bastion>>(_store.Values.ToList());

    public Task<Bastion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryGetValue(id, out Bastion? b) ? b : null);

    public Task AddAsync(Bastion bastion, CancellationToken cancellationToken = default)
    {
        _store[bastion.Id] = bastion;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Bastion bastion, CancellationToken cancellationToken = default)
    {
        _store[bastion.Id] = bastion;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.Remove(id);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

// ── Fake export service ──────────────────────────────────────────────────────

file sealed class FakeExportService : IExportService
{
    public string LastPublicJson { get; private set; } = string.Empty;
    public string LastDmJson { get; private set; } = string.Empty;

    public string ExportPublicJson(Bastion bastion) { LastPublicJson = "{}"; return "{}"; }
    public string ExportDmJson(Bastion bastion) { LastDmJson = "{}"; return "{}"; }

    public Task ExportPublicToFileAsync(Bastion bastion, string filePath, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task ExportDmToFileAsync(Bastion bastion, string filePath, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

// ── RelayCommand tests ───────────────────────────────────────────────────────

public class RelayCommandTests
{
    [Fact]
    public void Execute_CallsAction()
    {
        bool executed = false;
        var cmd = new RelayCommand(_ => executed = true);
        cmd.Execute(null);
        executed.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_DefaultsToTrue()
    {
        var cmd = new RelayCommand(_ => { });
        cmd.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithPredicate_ReturnsCorrectly()
    {
        bool enabled = false;
        var cmd = new RelayCommand(_ => { }, _ => enabled);

        cmd.CanExecute(null).Should().BeFalse();
        enabled = true;
        cmd.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void RaiseCanExecuteChanged_FiresEvent()
    {
        var cmd = new RelayCommand(_ => { });
        bool eventFired = false;
        cmd.CanExecuteChanged += (_, _) => eventFired = true;

        cmd.RaiseCanExecuteChanged();
        eventFired.Should().BeTrue();
    }
}

// ── NodeViewModel tests ──────────────────────────────────────────────────────

public class NodeViewModelTests
{
    [Fact]
    public void NodeViewModel_DiscoverabilityDC_ClampedTo0_20()
    {
        var node = new Node { DiscoverabilityDC = 10 };
        var vm = new NodeViewModel(node);

        vm.DiscoverabilityDC = 25;
        vm.DiscoverabilityDC.Should().Be(20);

        vm.DiscoverabilityDC = -5;
        vm.DiscoverabilityDC.Should().Be(0);
    }

    [Fact]
    public void NodeViewModel_PropertyChanged_FiresOnNameChange()
    {
        var vm = new NodeViewModel(new Node { Name = "Old" });
        string? changedProp = null;
        vm.PropertyChanged += (_, e) => changedProp = e.PropertyName;

        vm.Name = "New";
        changedProp.Should().Be(nameof(NodeViewModel.Name));
    }

    [Fact]
    public void NodeViewModel_AddFeature_AddsToCollection()
    {
        var vm = new NodeViewModel(new Node());
        var fVm = new FeatureViewModel(new Feature { Name = "Trap" });

        vm.AddFeature(fVm);
        vm.Features.Should().HaveCount(1);
    }

    [Fact]
    public void NodeViewModel_RemoveFeature_RemovesFromCollection()
    {
        var feature = new Feature { Name = "Trap" };
        var node = new Node();
        node.Features.Add(feature);
        var vm = new NodeViewModel(node);
        var fVm = vm.Features[0];

        vm.RemoveFeature(fVm);
        vm.Features.Should().BeEmpty();
    }
}

// ── EdgeViewModel tests ──────────────────────────────────────────────────────

public class EdgeViewModelTests
{
    [Fact]
    public void EdgeViewModel_PropertyChanged_FiresOnKindChange()
    {
        var vm = new EdgeViewModel(new Edge { Kind = EdgeKind.Door });
        string? changedProp = null;
        vm.PropertyChanged += (_, e) => changedProp = e.PropertyName;

        vm.Kind = EdgeKind.Window;
        changedProp.Should().Be(nameof(EdgeViewModel.Kind));
    }

    [Fact]
    public void EdgeViewModel_LockDC_CanBeSetAndCleared()
    {
        var vm = new EdgeViewModel(new Edge { HasLock = true, LockDC = 15 });
        vm.LockDC.Should().Be(15);

        vm.LockDC = null;
        vm.LockDC.Should().BeNull();
    }

    [Fact]
    public void EdgeViewModel_TrapFields_BindCorrectly()
    {
        var edge = new Edge
        {
            HasTrap = true,
            TrapKind = "poison dart",
            TrapDetectDC = 14,
            TrapDisarmDC = 16,
        };
        var vm = new EdgeViewModel(edge);

        vm.TrapKind.Should().Be("poison dart");
        vm.TrapDetectDC.Should().Be(14);
        vm.TrapDisarmDC.Should().Be(16);
    }
}

// ── WallSegmentViewModel tests ───────────────────────────────────────────────

public class WallSegmentViewModelTests
{
    [Fact]
    public void WallSegmentViewModel_EffectiveAC_IncludesReinforcements()
    {
        var seg = new WallSegment { BaseAC = 15 };
        seg.Reinforcements.Add(new Reinforcement { BonusAC = 5 });
        var vm = new WallSegmentViewModel(seg);

        vm.EffectiveAC.Should().Be(20);
    }

    [Fact]
    public void WallSegmentViewModel_HeightScaleFactor_IsComputed()
    {
        var seg = new WallSegment { HeightFeet = 20.0 };
        var vm = new WallSegmentViewModel(seg);

        vm.HeightScaleFactor.Should().BeApproximately(2.0, 0.001);
    }

    [Fact]
    public void WallSegmentViewModel_SolidFraction_ComputedFromOpeningSplits()
    {
        var seg = new WallSegment();
        seg.OpeningSplits.Add(new OpeningSplit { SplitPercentage = 0.3 });
        var vm = new WallSegmentViewModel(seg);

        vm.SolidFraction.Should().BeApproximately(0.7, 0.001);
    }
}

// ── MainViewModel tests ──────────────────────────────────────────────────────

public class MainViewModelTests
{
    private static MainViewModel CreateVm() =>
        new(new FakeBastionRepository(), new FakeExportService());

    [Fact]
    public void NewBastionCommand_CreatesBastion()
    {
        var vm = CreateVm();
        vm.NewBastionCommand.Execute(null);

        vm.BastionName.Should().Be("New Bastion");
    }

    [Fact]
    public void AddNodeCommand_AddsNodeToCollection()
    {
        var vm = CreateVm();
        vm.NewBastionCommand.Execute(null);
        vm.AddNodeCommand.Execute(null);

        vm.Nodes.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveNodeCommand_RemovesSelectedNode()
    {
        var vm = CreateVm();
        vm.NewBastionCommand.Execute(null);
        vm.AddNodeCommand.Execute(null);
        vm.SelectedNode = vm.Nodes[0];

        vm.RemoveNodeCommand.Execute(null);
        vm.Nodes.Should().BeEmpty();
    }

    [Fact]
    public void AddEdgeCommand_RequiresTwoNodes()
    {
        var vm = CreateVm();
        vm.NewBastionCommand.Execute(null);
        vm.AddNodeCommand.Execute(null);

        // Only one node: adding an edge should not add
        vm.AddEdgeCommand.CanExecute(null).Should().BeFalse();

        vm.AddNodeCommand.Execute(null);
        vm.AddEdgeCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void StatusMessage_IsSetOnNewBastion()
    {
        var vm = CreateVm();
        vm.NewBastionCommand.Execute(null);

        vm.StatusMessage.Should().Contain("New bastion");
    }
}
