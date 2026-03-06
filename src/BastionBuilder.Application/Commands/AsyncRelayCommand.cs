using System.Windows.Input;

namespace BastionBuilder.Application.Commands;

/// <summary>
/// An async relay command that wraps <see cref="Func{Task}"/> delegates,
/// preventing re-entrant execution and exposing an <c>IsExecuting</c> property.
/// </summary>
public sealed class AsyncRelayCommand : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object?, bool>? _canExecute;
    private bool _isExecuting;

    /// <summary>
    /// Initialises a new <see cref="AsyncRelayCommand"/>.
    /// </summary>
    public AsyncRelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>Whether an async execution is currently in progress.</summary>
    public bool IsExecuting => _isExecuting;

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>Raises <see cref="CanExecuteChanged"/> to trigger UI re-evaluation.</summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    public bool CanExecute(object? parameter)
        => !_isExecuting && (_canExecute is null || _canExecute(parameter));

    /// <inheritdoc />
    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        _isExecuting = true;
        RaiseCanExecuteChanged();
        try
        {
            await _execute(parameter);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }
}
