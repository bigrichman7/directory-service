using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(dp => dp.Id).HasName("pk_department_positions");

        builder
            .Property(dp => dp.Id)
            .HasConversion(dp => dp.Value, dp => new DepartmentPositionId(dp))
            .HasColumnName("id");

        builder
            .Property(dp => dp.DepartmentId)
            .HasConversion(d => d.Value, d => new DepartmentId(d))
            .HasColumnName("department_id");

        builder
            .Property(dp => dp.PositionId)
            .HasConversion(d => d.Value, d => new PositionId(d))
            .HasColumnName("position_id");

        builder
            .HasOne(dp => dp.Department)
            .WithMany(d => d.DepartmentPositions)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(dp => dp.Position)
            .WithMany(p => p.DepartmentPositions)
            .HasForeignKey(p => p.PositionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


