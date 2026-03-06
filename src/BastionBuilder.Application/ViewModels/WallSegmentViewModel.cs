using System.Collections.ObjectModel;
using BastionBuilder.Domain.Entities;

namespace BastionBuilder.Application.ViewModels;

/// <summary>ViewModel wrapper around a <see cref="WallSegment"/> entity.</summary>
public class WallSegmentViewModel : ViewModelBase
{
    private readonly WallSegment _segment;

    public WallSegmentViewModel(WallSegment segment)
    {
        _segment = segment ?? throw new ArgumentNullException(nameof(segment));
    }

    public Guid Id => _segment.Id;

    public string Material
    {
        get => _segment.Material;
        set { if (_segment.Material != value) { _segment.Material = value; OnPropertyChanged(); } }
    }

    public int BaseAC
    {
        get => _segment.BaseAC;
        set { if (_segment.BaseAC != value) { _segment.BaseAC = value; OnPropertyChanged(); OnPropertyChanged(nameof(EffectiveAC)); } }
    }

    public int BaseHP
    {
        get => _segment.BaseHP;
        set { if (_segment.BaseHP != value) { _segment.BaseHP = value; OnPropertyChanged(); OnPropertyChanged(nameof(EffectiveHP)); } }
    }

    public double HeightFeet
    {
        get => _segment.HeightFeet;
        set { if (_segment.HeightFeet != value) { _segment.HeightFeet = value; OnPropertyChanged(); OnPropertyChanged(nameof(HeightScaleFactor)); } }
    }

    public double LengthFeet
    {
        get => _segment.LengthFeet;
        set { if (_segment.LengthFeet != value) { _segment.LengthFeet = value; OnPropertyChanged(); } }
    }

    public double ThicknessInches
    {
        get => _segment.ThicknessInches;
        set { if (_segment.ThicknessInches != value) { _segment.ThicknessInches = value; OnPropertyChanged(); } }
    }

    /// <summary>Effective AC (base + all reinforcement bonuses).</summary>
    public int EffectiveAC => Rules.WallSegmentRules.EffectiveAC(_segment);

    /// <summary>Effective HP (base + all reinforcement bonuses).</summary>
    public int EffectiveHP => Rules.WallSegmentRules.EffectiveHP(_segment);

    /// <summary>Height scale factor from height-scaling rules stub.</summary>
    public double HeightScaleFactor => Rules.WallSegmentRules.HeightScaleFactor(_segment);

    /// <summary>Fraction of the segment that is solid (0–1).</summary>
    public double SolidFraction => Rules.WallSegmentRules.SolidFraction(_segment);

    /// <summary>Returns the underlying domain entity.</summary>
    public WallSegment ToDomain() => _segment;
}
