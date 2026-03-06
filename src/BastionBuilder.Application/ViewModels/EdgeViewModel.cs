using System.Collections.ObjectModel;
using BastionBuilder.Domain.Entities;
using BastionBuilder.Domain.Enums;

namespace BastionBuilder.Application.ViewModels;

/// <summary>ViewModel wrapper around an <see cref="Edge"/> entity.</summary>
public class EdgeViewModel : ViewModelBase
{
    private readonly Edge _edge;

    public EdgeViewModel(Edge edge)
    {
        _edge = edge ?? throw new ArgumentNullException(nameof(edge));
        Features = new ObservableCollection<FeatureViewModel>(
            edge.Features.Select(f => new FeatureViewModel(f)));
    }

    public Guid Id => _edge.Id;

    public string Name
    {
        get => _edge.Name;
        set { if (_edge.Name != value) { _edge.Name = value; OnPropertyChanged(); } }
    }

    public string Description
    {
        get => _edge.Description;
        set { if (_edge.Description != value) { _edge.Description = value; OnPropertyChanged(); } }
    }

    public string PublicDescription
    {
        get => _edge.PublicDescription;
        set { if (_edge.PublicDescription != value) { _edge.PublicDescription = value; OnPropertyChanged(); } }
    }

    public EdgeKind Kind
    {
        get => _edge.Kind;
        set { if (_edge.Kind != value) { _edge.Kind = value; OnPropertyChanged(); } }
    }

    public Guid FromNodeId
    {
        get => _edge.FromNodeId;
        set { if (_edge.FromNodeId != value) { _edge.FromNodeId = value; OnPropertyChanged(); } }
    }

    public Guid ToNodeId
    {
        get => _edge.ToNodeId;
        set { if (_edge.ToNodeId != value) { _edge.ToNodeId = value; OnPropertyChanged(); } }
    }

    public int DiscoverabilityDC
    {
        get => _edge.DiscoverabilityDC;
        set
        {
            int clamped = Math.Clamp(value, 0, 20);
            if (_edge.DiscoverabilityDC != clamped) { _edge.DiscoverabilityDC = clamped; OnPropertyChanged(); }
        }
    }

    public bool IsSecret
    {
        get => _edge.IsSecret;
        set { if (_edge.IsSecret != value) { _edge.IsSecret = value; OnPropertyChanged(); } }
    }

    public VisibilityState VisibilityState
    {
        get => _edge.VisibilityState;
        set { if (_edge.VisibilityState != value) { _edge.VisibilityState = value; OnPropertyChanged(); } }
    }

    // ── Lock ────────────────────────────────────────────────────────────────

    public bool HasLock
    {
        get => _edge.HasLock;
        set { if (_edge.HasLock != value) { _edge.HasLock = value; OnPropertyChanged(); } }
    }

    public int? LockDC
    {
        get => _edge.LockDC;
        set { if (_edge.LockDC != value) { _edge.LockDC = value; OnPropertyChanged(); } }
    }

    public string? LockDescription
    {
        get => _edge.LockDescription;
        set { if (_edge.LockDescription != value) { _edge.LockDescription = value; OnPropertyChanged(); } }
    }

    // ── Trap ────────────────────────────────────────────────────────────────

    public bool HasTrap
    {
        get => _edge.HasTrap;
        set { if (_edge.HasTrap != value) { _edge.HasTrap = value; OnPropertyChanged(); } }
    }

    public string? TrapKind
    {
        get => _edge.TrapKind;
        set { if (_edge.TrapKind != value) { _edge.TrapKind = value; OnPropertyChanged(); } }
    }

    public int? TrapDetectDC
    {
        get => _edge.TrapDetectDC;
        set { if (_edge.TrapDetectDC != value) { _edge.TrapDetectDC = value; OnPropertyChanged(); } }
    }

    public int? TrapDisarmDC
    {
        get => _edge.TrapDisarmDC;
        set { if (_edge.TrapDisarmDC != value) { _edge.TrapDisarmDC = value; OnPropertyChanged(); } }
    }

    public string? TrapEffect
    {
        get => _edge.TrapEffect;
        set { if (_edge.TrapEffect != value) { _edge.TrapEffect = value; OnPropertyChanged(); } }
    }

    // ── Alarm ───────────────────────────────────────────────────────────────

    public bool HasAlarm
    {
        get => _edge.HasAlarm;
        set { if (_edge.HasAlarm != value) { _edge.HasAlarm = value; OnPropertyChanged(); } }
    }

    public int? AlarmThreshold
    {
        get => _edge.AlarmThreshold;
        set { if (_edge.AlarmThreshold != value) { _edge.AlarmThreshold = value; OnPropertyChanged(); } }
    }

    public string? AlarmDescription
    {
        get => _edge.AlarmDescription;
        set { if (_edge.AlarmDescription != value) { _edge.AlarmDescription = value; OnPropertyChanged(); } }
    }

    // ── Physical ────────────────────────────────────────────────────────────

    public double HeightFeet
    {
        get => _edge.HeightFeet;
        set { if (_edge.HeightFeet != value) { _edge.HeightFeet = value; OnPropertyChanged(); } }
    }

    public double WidthFeet
    {
        get => _edge.WidthFeet;
        set { if (_edge.WidthFeet != value) { _edge.WidthFeet = value; OnPropertyChanged(); } }
    }

    public bool IsPassable
    {
        get => _edge.IsPassable;
        set { if (_edge.IsPassable != value) { _edge.IsPassable = value; OnPropertyChanged(); } }
    }

    public ObservableCollection<FeatureViewModel> Features { get; }

    /// <summary>Returns the underlying domain entity.</summary>
    public Edge ToDomain() => _edge;
}
