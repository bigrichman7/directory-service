using DirectoryService.Domain.Positions;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id).HasName("pk_positions");

        builder
            .Property(p => p.Id)
            .HasConversion(p => p.Value, p => new PositionId(p))
            .HasColumnName("id");

        builder.ComplexProperty(p => p.Name, nb =>
        {
            nb.IsRequired();

            nb.Property(v => v.Value)
            .IsRequired(true)
            .HasMaxLength(Name.MAX_LENGTH)
            .HasColumnName("name");
        });

        builder
            .Property(p => p.CreatedAt)
            .HasColumnName ("created_at");

        builder
            .Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");
    }
}


