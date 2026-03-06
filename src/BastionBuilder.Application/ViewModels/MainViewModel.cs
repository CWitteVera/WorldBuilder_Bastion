using System.Collections.ObjectModel;
using BastionBuilder.Application.Commands;
using BastionBuilder.Application.Interfaces;
using BastionBuilder.Domain.Entities;

namespace BastionBuilder.Application.ViewModels;

/// <summary>
/// Primary ViewModel for the main window.  Manages the list of bastions and
/// the currently selected bastion with its nodes and edges.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly IBastionRepository _repository;
    private readonly IExportService _exportService;

    private Bastion? _currentBastion;
    private NodeViewModel? _selectedNode;
    private EdgeViewModel? _selectedEdge;
    private string _statusMessage = "Ready";
    private bool _isBusy;

    public MainViewModel(IBastionRepository repository, IExportService exportService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));

        Nodes = [];
        Edges = [];

        LoadBastionCommand = new AsyncRelayCommand(LoadBastionAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync, _ => _currentBastion is not null);
        AddNodeCommand = new RelayCommand(AddNode, _ => _currentBastion is not null);
        RemoveNodeCommand = new RelayCommand(RemoveNode, _ => SelectedNode is not null);
        AddEdgeCommand = new RelayCommand(AddEdge, _ => _currentBastion is not null && Nodes.Count >= 2);
        RemoveEdgeCommand = new RelayCommand(RemoveEdge, _ => SelectedEdge is not null);
        ExportPublicCommand = new AsyncRelayCommand(ExportPublicAsync, _ => _currentBastion is not null);
        ExportDmCommand = new AsyncRelayCommand(ExportDmAsync, _ => _currentBastion is not null);
        NewBastionCommand = new RelayCommand(NewBastion);
    }

    // ── Properties ──────────────────────────────────────────────────────────

    public string BastionName
    {
        get => _currentBastion?.Name ?? string.Empty;
        set
        {
            if (_currentBastion is not null && _currentBastion.Name != value)
            {
                _currentBastion.Name = value;
                OnPropertyChanged();
            }
        }
    }

    public string BastionDescription
    {
        get => _currentBastion?.Description ?? string.Empty;
        set
        {
            if (_currentBastion is not null && _currentBastion.Description != value)
            {
                _currentBastion.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<NodeViewModel> Nodes { get; }
    public ObservableCollection<EdgeViewModel> Edges { get; }

    public NodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set
        {
            if (SetProperty(ref _selectedNode, value))
            {
                ((RelayCommand)RemoveNodeCommand).RaiseCanExecuteChanged();
                ((RelayCommand)AddEdgeCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public EdgeViewModel? SelectedEdge
    {
        get => _selectedEdge;
        set
        {
            if (SetProperty(ref _selectedEdge, value))
            {
                ((RelayCommand)RemoveEdgeCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    // ── Commands ────────────────────────────────────────────────────────────

    public AsyncRelayCommand LoadBastionCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public RelayCommand AddNodeCommand { get; }
    public RelayCommand RemoveNodeCommand { get; }
    public RelayCommand AddEdgeCommand { get; }
    public RelayCommand RemoveEdgeCommand { get; }
    public AsyncRelayCommand ExportPublicCommand { get; }
    public AsyncRelayCommand ExportDmCommand { get; }
    public RelayCommand NewBastionCommand { get; }

    // ── Command implementations ─────────────────────────────────────────────

    private async Task LoadBastionAsync(object? parameter)
    {
        IsBusy = true;
        StatusMessage = "Loading…";
        try
        {
            Guid? id = parameter as Guid?;
            if (id is null && parameter is Guid g) id = g;

            Bastion? loaded = id.HasValue
                ? await _repository.GetByIdAsync(id.Value)
                : null;

            if (loaded is not null) LoadBastion(loaded);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Load failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync(object? _)
    {
        if (_currentBastion is null) return;
        IsBusy = true;
        StatusMessage = "Saving…";
        try
        {
            await _repository.UpdateAsync(_currentBastion);
            await _repository.SaveChangesAsync();
            StatusMessage = "Saved.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AddNode(object? _)
    {
        if (_currentBastion is null) return;
        var node = new Node { Name = "New Room" };
        _currentBastion.Nodes.Add(node);
        Nodes.Add(new NodeViewModel(node));
        ((RelayCommand)AddEdgeCommand).RaiseCanExecuteChanged();
        StatusMessage = $"Added node \"{node.Name}\".";
    }

    private void RemoveNode(object? _)
    {
        if (SelectedNode is null || _currentBastion is null) return;
        _currentBastion.Nodes.Remove(SelectedNode.ToDomain());
        Nodes.Remove(SelectedNode);
        SelectedNode = null;
        StatusMessage = "Node removed.";
    }

    private void AddEdge(object? _)
    {
        if (_currentBastion is null || Nodes.Count < 2) return;
        var edge = new Edge
        {
            Name = "New Door",
            FromNodeId = Nodes[0].Id,
            ToNodeId = Nodes[1].Id,
        };
        _currentBastion.Edges.Add(edge);
        Edges.Add(new EdgeViewModel(edge));
        StatusMessage = $"Added edge \"{edge.Name}\".";
    }

    private void RemoveEdge(object? _)
    {
        if (SelectedEdge is null || _currentBastion is null) return;
        _currentBastion.Edges.Remove(SelectedEdge.ToDomain());
        Edges.Remove(SelectedEdge);
        SelectedEdge = null;
        StatusMessage = "Edge removed.";
    }

    private async Task ExportPublicAsync(object? _)
    {
        if (_currentBastion is null) return;
        IsBusy = true;
        StatusMessage = "Exporting (public)…";
        try
        {
            string path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"{_currentBastion.Name}_public_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            await _exportService.ExportPublicToFileAsync(_currentBastion, path);
            StatusMessage = $"Public export saved to {path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExportDmAsync(object? _)
    {
        if (_currentBastion is null) return;
        IsBusy = true;
        StatusMessage = "Exporting (DM)…";
        try
        {
            string path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"{_currentBastion.Name}_dm_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            await _exportService.ExportDmToFileAsync(_currentBastion, path);
            StatusMessage = $"DM export saved to {path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void NewBastion(object? _)
    {
        LoadBastion(new Bastion { Name = "New Bastion" });
        StatusMessage = "New bastion created. Remember to save.";
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private void LoadBastion(Bastion bastion)
    {
        _currentBastion = bastion;

        Nodes.Clear();
        foreach (Node n in bastion.Nodes)
            Nodes.Add(new NodeViewModel(n));

        Edges.Clear();
        foreach (Edge e in bastion.Edges)
            Edges.Add(new EdgeViewModel(e));

        OnPropertyChanged(nameof(BastionName));
        OnPropertyChanged(nameof(BastionDescription));

        ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
        ((RelayCommand)AddNodeCommand).RaiseCanExecuteChanged();
        ((RelayCommand)AddEdgeCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)ExportPublicCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)ExportDmCommand).RaiseCanExecuteChanged();
    }
}
