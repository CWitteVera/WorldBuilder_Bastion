using BastionBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BastionBuilder.Persistence.Sqlite.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Description).HasMaxLength(4000);
        builder.Property(f => f.PublicDescription).HasMaxLength(4000);
        builder.Property(f => f.DiscoverabilityDC).HasDefaultValue(0);

        // RevealPropagatesTo stored as pipe-delimited GUIDs
        builder.Property(f => f.RevealPropagatesTo)
               .HasConversion(
                   v => string.Join('|', v.Select(g => g.ToString())),
                   v => v.Length == 0
                       ? new List<Guid>()
                       : v.Split('|', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Guid.Parse)
                          .ToList());
    }
}
