using BastionBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BastionBuilder.Persistence.Sqlite.Configurations;

public class WallGroupConfiguration : IEntityTypeConfiguration<WallGroup>
{
    public void Configure(EntityTypeBuilder<WallGroup> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Description).HasMaxLength(4000);

        builder.HasMany(w => w.Segments)
               .WithOne()
               .HasForeignKey(s => s.WallGroupId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WallSegmentConfiguration : IEntityTypeConfiguration<WallSegment>
{
    public void Configure(EntityTypeBuilder<WallSegment> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Material).HasMaxLength(200);
        builder.Property(s => s.BaseAC).HasDefaultValue(15);
        builder.Property(s => s.BaseHP).HasDefaultValue(27);
        builder.Property(s => s.HeightFeet).HasDefaultValue(10.0);
        builder.Property(s => s.ThicknessInches).HasDefaultValue(12.0);

        // SharedWithNodeIds stored as pipe-delimited GUIDs
        builder.Property(s => s.SharedWithNodeIds)
               .HasConversion(
                   v => string.Join('|', v.Select(g => g.ToString())),
                   v => v.Length == 0
                       ? new List<Guid>()
                       : v.Split('|', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Guid.Parse)
                          .ToList());

        builder.HasMany(s => s.Reinforcements)
               .WithOne()
               .HasForeignKey(r => r.WallSegmentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.OpeningSplits)
               .WithOne()
               .HasForeignKey(o => o.WallSegmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReinforcementConfiguration : IEntityTypeConfiguration<Reinforcement>
{
    public void Configure(EntityTypeBuilder<Reinforcement> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Description).HasMaxLength(500);
    }
}

public class OpeningSplitConfiguration : IEntityTypeConfiguration<OpeningSplit>
{
    public void Configure(EntityTypeBuilder<OpeningSplit> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Description).HasMaxLength(500);
    }
}
