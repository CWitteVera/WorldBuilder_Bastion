using BastionBuilder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BastionBuilder.Persistence.Sqlite.Configurations;

public class BastionConfiguration : IEntityTypeConfiguration<Bastion>
{
    public void Configure(EntityTypeBuilder<Bastion> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Description).HasMaxLength(4000);

        builder.HasMany(b => b.Nodes)
               .WithOne()
               .HasForeignKey("BastionId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Edges)
               .WithOne()
               .HasForeignKey("BastionId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.WallGroups)
               .WithOne()
               .HasForeignKey("BastionId")
               .OnDelete(DeleteBehavior.Cascade);
    }
}
