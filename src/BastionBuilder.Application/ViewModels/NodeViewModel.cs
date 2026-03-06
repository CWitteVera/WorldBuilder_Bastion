using System.Collections.ObjectModel;
using BastionBuilder.Domain.Entities;

namespace BastionBuilder.Application.ViewModels;

/// <summary>ViewModel wrapper around a <see cref="Node"/> entity.</summary>
public class NodeViewModel : ViewModelBase
{
    private readonly Node _node;

    public NodeViewModel(Node node)
    {
        _node = node ?? throw new ArgumentNullException(nameof(node));
        Features = new ObservableCollection<FeatureViewModel>(
            node.Features.Select(f => new FeatureViewModel(f)));
    }

    public Guid Id => _node.Id;

    public string Name
    {
        get => _node.Name;
        set { if (_node.Name != value) { _node.Name = value; OnPropertyChanged(); } }
    }

    public string Description
    {
        get => _node.Description;
        set { if (_node.Description != value) { _node.Description = value; OnPropertyChanged(); } }
    }

    public string PublicDescription
    {
        get => _node.PublicDescription;
        set { if (_node.PublicDescription != value) { _node.PublicDescription = value; OnPropertyChanged(); } }
    }

    public int DiscoverabilityDC
    {
        get => _node.DiscoverabilityDC;
        set
        {
            int clamped = Math.Clamp(value, 0, 20);
            if (_node.DiscoverabilityDC != clamped)
            {
                _node.DiscoverabilityDC = clamped;
                OnPropertyChanged();
            }
        }
    }

    public double HeightFeet
    {
        get => _node.HeightFeet;
        set { if (_node.HeightFeet != value) { _node.HeightFeet = value; OnPropertyChanged(); } }
    }

    public double AreaSquareFeet
    {
        get => _node.AreaSquareFeet;
        set { if (_node.AreaSquareFeet != value) { _node.AreaSquareFeet = value; OnPropertyChanged(); } }
    }

    public double PositionX
    {
        get => _node.PositionX;
        set { if (_node.PositionX != value) { _node.PositionX = value; OnPropertyChanged(); } }
    }

    public double PositionY
    {
        get => _node.PositionY;
        set { if (_node.PositionY != value) { _node.PositionY = value; OnPropertyChanged(); } }
    }

    public int FloorLevel
    {
        get => _node.FloorLevel;
        set { if (_node.FloorLevel != value) { _node.FloorLevel = value; OnPropertyChanged(); } }
    }

    public ObservableCollection<FeatureViewModel> Features { get; }

    public void AddFeature(FeatureViewModel vm)
    {
        _node.Features.Add(vm.ToDomain());
        Features.Add(vm);
    }

    public void RemoveFeature(FeatureViewModel vm)
    {
        _node.Features.Remove(vm.ToDomain());
        Features.Remove(vm);
    }

    /// <summary>Returns the underlying domain entity.</summary>
    public Node ToDomain() => _node;
}
