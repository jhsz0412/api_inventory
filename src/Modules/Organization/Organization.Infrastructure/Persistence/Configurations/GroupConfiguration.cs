using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Persistence.Configurations;

public sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> entity)
    {
        entity.ToTable("Groups", "Organization");

        entity.HasKey(group => group.Id);

        entity.Property(group => group.Code)
            .HasMaxLength(30)
            .IsRequired();

        entity.Property(group => group.Name)
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(group => group.DepartmentId)
            .IsRequired();

        entity.Property(group => group.IsActive)
            .IsRequired();

        entity.Property(group => group.CreatedAtUtc)
            .IsRequired();

        entity.HasOne<Department>()
            .WithMany()
            .HasForeignKey(group => group.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(group => new { group.DepartmentId, group.Code })
            .IsUnique();
    }
}