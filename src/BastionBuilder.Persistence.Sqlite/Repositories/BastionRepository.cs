using BastionBuilder.Application.Interfaces;
using BastionBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BastionBuilder.Persistence.Sqlite.Repositories;

/// <summary>
/// SQLite-backed implementation of <see cref="IBastionRepository"/> using EF Core.
/// </summary>
public class BastionRepository : IBastionRepository
{
    private readonly BastionDbContext _context;

    public BastionRepository(BastionDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Bastion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bastions
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Bastion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bastions
            .Include(b => b.Nodes).ThenInclude(n => n.Features)
            .Include(b => b.Edges).ThenInclude(e => e.Features)
            .Include(b => b.WallGroups)
                .ThenInclude(wg => wg.Segments)
                    .ThenInclude(s => s.Reinforcements)
            .Include(b => b.WallGroups)
                .ThenInclude(wg => wg.Segments)
                    .ThenInclude(s => s.OpeningSplits)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Bastion bastion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bastion);
        bastion.CreatedAtUtc = DateTime.UtcNow;
        bastion.ModifiedAtUtc = DateTime.UtcNow;
        await _context.Bastions.AddAsync(bastion, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Bastion bastion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bastion);
        bastion.ModifiedAtUtc = DateTime.UtcNow;
        _context.Bastions.Update(bastion);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Bastion? bastion = await _context.Bastions.FindAsync([id], cancellationToken);
        if (bastion is not null)
            _context.Bastions.Remove(bastion);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
