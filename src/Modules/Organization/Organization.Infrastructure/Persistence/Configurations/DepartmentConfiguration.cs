using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Persistence.Configurations;

public sealed class DepartmentConfiguration
    : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> entity)
    {
        entity.ToTable("Departments", "Organization");

        entity.HasKey(department => department.Id);

        entity.Property(department => department.Code)
            .HasMaxLength(30)
            .IsRequired();

        entity.Property(department => department.Name)
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(department => department.IsActive)
            .IsRequired();

        entity.Property(department => department.CreatedAtUtc)
            .IsRequired();

        entity.HasIndex(department => department.Code)
            .IsUnique();
    }
}