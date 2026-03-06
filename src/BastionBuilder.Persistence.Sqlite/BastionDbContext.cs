using BastionBuilder.Domain.Entities;
using BastionBuilder.Persistence.Sqlite.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BastionBuilder.Persistence.Sqlite;

/// <summary>
/// Entity Framework Core DbContext for BastionBuilder.
/// Uses SQLite as the backing store.
/// </summary>
public class BastionDbContext : DbContext
{
    public BastionDbContext(DbContextOptions<BastionDbContext> options) : base(options) { }

    public DbSet<Bastion> Bastions => Set<Bastion>();
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<Edge> Edges => Set<Edge>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<WallGroup> WallGroups => Set<WallGroup>();
    public DbSet<WallSegment> WallSegments => Set<WallSegment>();
    public DbSet<Reinforcement> Reinforcements => Set<Reinforcement>();
    public DbSet<OpeningSplit> OpeningSplits => Set<OpeningSplit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BastionConfiguration());
        modelBuilder.ApplyConfiguration(new NodeConfiguration());
        modelBuilder.ApplyConfiguration(new EdgeConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureConfiguration());
        modelBuilder.ApplyConfiguration(new WallGroupConfiguration());
        modelBuilder.ApplyConfiguration(new WallSegmentConfiguration());
        modelBuilder.ApplyConfiguration(new ReinforcementConfiguration());
        modelBuilder.ApplyConfiguration(new OpeningSplitConfiguration());
    }
}
