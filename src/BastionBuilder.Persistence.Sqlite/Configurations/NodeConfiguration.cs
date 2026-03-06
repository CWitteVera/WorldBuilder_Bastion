using BastionBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BastionBuilder.Persistence.Sqlite.Configurations;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Name).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Description).HasMaxLength(4000);
        builder.Property(n => n.PublicDescription).HasMaxLength(4000);
        builder.Property(n => n.DiscoverabilityDC).HasDefaultValue(0);
        builder.Property(n => n.HeightFeet).HasDefaultValue(10.0);

        // Tags stored as a pipe-delimited string
        builder.Property(n => n.Tags)
               .HasConversion(
                   v => string.Join('|', v),
                   v => v.Length == 0 ? new List<string>() : v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());

        builder.HasMany(n => n.Features)
               .WithOne()
               .HasForeignKey(f => f.NodeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
