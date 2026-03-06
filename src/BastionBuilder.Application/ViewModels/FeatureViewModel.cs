using BastionBuilder.Domain.Entities;
using BastionBuilder.Domain.Enums;

namespace BastionBuilder.Application.ViewModels;

/// <summary>ViewModel wrapper around a <see cref="Feature"/> entity.</summary>
public class FeatureViewModel : ViewModelBase
{
    private readonly Feature _feature;

    public FeatureViewModel(Feature feature)
    {
        _feature = feature ?? throw new ArgumentNullException(nameof(feature));
    }

    public Guid Id => _feature.Id;

    public string Name
    {
        get => _feature.Name;
        set { if (_feature.Name != value) { _feature.Name = value; OnPropertyChanged(); } }
    }

    public string Description
    {
        get => _feature.Description;
        set { if (_feature.Description != value) { _feature.Description = value; OnPropertyChanged(); } }
    }

    public string PublicDescription
    {
        get => _feature.PublicDescription;
        set { if (_feature.PublicDescription != value) { _feature.PublicDescription = value; OnPropertyChanged(); } }
    }

    public bool IsSecret
    {
        get => _feature.IsSecret;
        set { if (_feature.IsSecret != value) { _feature.IsSecret = value; OnPropertyChanged(); } }
    }

    public VisibilityState VisibilityState
    {
        get => _feature.VisibilityState;
        set { if (_feature.VisibilityState != value) { _feature.VisibilityState = value; OnPropertyChanged(); } }
    }

    public int DiscoverabilityDC
    {
        get => _feature.DiscoverabilityDC;
        set
        {
            int clamped = Math.Clamp(value, 0, 20);
            if (_feature.DiscoverabilityDC != clamped)
            {
                _feature.DiscoverabilityDC = clamped;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>Returns the underlying domain entity.</summary>
    public Feature ToDomain() => _feature;
}
