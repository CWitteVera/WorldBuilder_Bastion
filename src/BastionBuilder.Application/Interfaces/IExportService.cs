using BastionBuilder.Domain.Entities;

namespace BastionBuilder.Application.Interfaces;

/// <summary>
/// Export service that serialises a <see cref="Bastion"/> to JSON in either
/// the public (player-visible) or full DM view.
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exports the bastion as a public JSON document.
    /// Secret elements that have not been revealed are omitted or redacted.
    /// </summary>
    /// <param name="bastion">The bastion to export.</param>
    /// <returns>UTF-8 JSON string.</returns>
    string ExportPublicJson(Bastion bastion);

    /// <summary>
    /// Exports the bastion as a full DM JSON document.
    /// All elements including unrevealed secrets are included.
    /// </summary>
    /// <param name="bastion">The bastion to export.</param>
    /// <returns>UTF-8 JSON string.</returns>
    string ExportDmJson(Bastion bastion);

    /// <summary>
    /// Writes the public JSON export to the specified file path.
    /// </summary>
    Task ExportPublicToFileAsync(Bastion bastion, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the full DM JSON export to the specified file path.
    /// </summary>
    Task ExportDmToFileAsync(Bastion bastion, string filePath, CancellationToken cancellationToken = default);
}
