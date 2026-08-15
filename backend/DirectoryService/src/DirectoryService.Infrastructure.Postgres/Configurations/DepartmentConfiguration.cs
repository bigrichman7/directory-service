using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id).HasName("pk_departments");

        builder.Property(d => d.Id)
            .HasConversion(v => v.Value, v => new DepartmentId(v))
            .HasColumnName("id");

        builder.ComplexProperty(d => d.Name, nb =>
        {
            nb.IsRequired();

            nb.Property(v => v.Value)
            .IsRequired(true)
            .HasMaxLength(Name.MAX_LENGTH)
            .HasColumnName("name");
        });

        builder.ComplexProperty(d => d.Slug, nb =>
        {
            nb.IsRequired();

            nb.Property(v => v.Value)
                .HasMaxLength(Slug.MAX_LENGTH)
                .HasColumnName("slug");
        });

        builder.ComplexProperty(d => d.Path, nb =>
        {
            nb.IsRequired();

            nb.Property(v => v.Value)
                .HasMaxLength(Slug.MAX_LENGTH)
                .HasColumnName("path");
        });

        builder.Property(d => d.ParentId)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v.HasValue ? new DepartmentId(v.Value) : null)
            .HasColumnName("parent_id");

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at");
    }
}


