using System.Windows.Input;

namespace BastionBuilder.Application.Commands;

/// <summary>
/// A simple relay command that wraps <see cref="Action"/> and optional
/// <see cref="Func{Boolean}"/> delegates for <c>Execute</c> and <c>CanExecute</c>.
/// </summary>
public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Initialises a new <see cref="RelayCommand"/>.
    /// </summary>
    /// <param name="execute">Action to run when the command executes.</param>
    /// <param name="canExecute">Optional predicate controlling whether the command is enabled.</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>Raises <see cref="CanExecuteChanged"/> to trigger UI re-evaluation.</summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute(parameter);

    /// <inheritdoc />
    public void Execute(object? parameter) => _execute(parameter);
}
