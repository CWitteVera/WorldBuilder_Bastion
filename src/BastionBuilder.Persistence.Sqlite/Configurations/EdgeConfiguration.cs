using BastionBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BastionBuilder.Persistence.Sqlite.Configurations;

public class EdgeConfiguration : IEntityTypeConfiguration<Edge>
{
    public void Configure(EntityTypeBuilder<Edge> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.PublicDescription).HasMaxLength(4000);
        builder.Property(e => e.TrapKind).HasMaxLength(200);
        builder.Property(e => e.TrapEffect).HasMaxLength(2000);
        builder.Property(e => e.LockDescription).HasMaxLength(500);
        builder.Property(e => e.AlarmDescription).HasMaxLength(500);
        builder.Property(e => e.DiscoverabilityDC).HasDefaultValue(0);
        builder.Property(e => e.HeightFeet).HasDefaultValue(7.0);
        builder.Property(e => e.WidthFeet).HasDefaultValue(5.0);

        builder.HasMany(e => e.Features)
               .WithOne()
               .HasForeignKey(f => f.EdgeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
