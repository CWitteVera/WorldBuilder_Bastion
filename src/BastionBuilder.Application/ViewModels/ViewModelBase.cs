using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BastionBuilder.Application.ViewModels;

/// <summary>
/// Base class for all ViewModels. Implements <see cref="INotifyPropertyChanged"/>
/// so that WPF data bindings update automatically when properties change.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises <see cref="PropertyChanged"/> for the calling member.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed (auto-filled by compiler).</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Sets <paramref name="field"/> to <paramref name="value"/> and raises
    /// <see cref="PropertyChanged"/> if the value actually changed.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="field">Backing field reference.</param>
    /// <param name="value">New value.</param>
    /// <param name="propertyName">Property name (auto-filled by compiler).</param>
    /// <returns><c>true</c> if the value changed.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
