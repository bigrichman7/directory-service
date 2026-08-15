using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");

        builder.HasKey(dl => dl.Id).HasName("pk_department_locations");

        builder
            .Property(dl => dl.Id)
            .HasConversion(dl => dl.Value, dl => new DepartmentLocationId(dl))
            .HasColumnName("id");

        builder.Property(dl => dl.IsPrimary)
            .HasColumnName("is_primary");

        builder
            .Property(dl => dl.DepartmentId)
            .HasConversion(dl => dl.Value, dl => new DepartmentId(dl))
            .HasColumnName("department_id");
        
        builder
            .Property(dl => dl.LocationId)
            .HasConversion(dl => dl.Value, dl => new LocationId(dl))
            .HasColumnName("location_id");

        builder
            .HasOne(dl => dl.Department)
            .WithMany(d => d.DepartmentLocations)
            .HasForeignKey(dl => dl.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(dl => dl.Location)
            .WithMany(l => l.DepartmentLocations)
            .HasForeignKey(dl => dl.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


