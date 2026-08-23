using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id).HasName("pk_locations");

        builder
            .Property(l => l.Id)
            .HasConversion(l => l.Value, l => new LocationId(l))
            .HasColumnName("id");

        builder.ComplexProperty(l => l.Name, nb =>
        {
            nb.IsRequired();

            nb.Property(v => v.Value)
            .IsRequired(true)
            .HasMaxLength(Name.MAX_LENGTH)
            .HasColumnName("name");
        });

        builder.ComplexProperty(l => l.Address, nb =>
        {
            nb.IsRequired();

            nb.Property(v => v.City)
            .IsRequired(true)
            .HasMaxLength(Address.MAX_LENGTH)
            .HasColumnName("city");

            nb.Property(v => v.Street)
            .IsRequired(true)
            .HasMaxLength(Address.MAX_LENGTH)
            .HasColumnName("street");

            nb.Property(v => v.House)
            .IsRequired(true)
            .HasMaxLength(Address.MAX_LENGTH)
            .HasColumnName("house");

            nb.Property(v => v.Apartment)
            .IsRequired(true)
            .HasMaxLength(Address.MAX_LENGTH)
            .HasColumnName("apartment");
        });

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at");

    }
}


