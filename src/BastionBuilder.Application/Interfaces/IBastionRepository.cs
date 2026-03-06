using BastionBuilder.Domain.Entities;

namespace BastionBuilder.Application.Interfaces;

/// <summary>
/// Repository abstraction for persisting and querying <see cref="Bastion"/> aggregates.
/// </summary>
public interface IBastionRepository
{
    /// <summary>Returns all bastions (summary data only).</summary>
    Task<IReadOnlyList<Bastion>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single bastion by ID with all related data, or null if not found.</summary>
    Task<Bastion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists a new bastion.</summary>
    Task AddAsync(Bastion bastion, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing bastion.</summary>
    Task UpdateAsync(Bastion bastion, CancellationToken cancellationToken = default);

    /// <summary>Removes a bastion and all its related data.</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists any pending changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
