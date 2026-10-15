
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Persistence.Configurations;

public sealed class PositionConfiguration
    : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> entity)
    {
        entity.ToTable("Positions", "Organization");

        entity.HasKey(position => position.Id);

        entity.Property(position => position.Code)
            .HasMaxLength(30)
            .IsRequired();

        entity.Property(position => position.Name)
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(position => position.IsActive)
            .IsRequired();

        entity.Property(position => position.CreatedAtUtc)
            .IsRequired();

        entity.HasIndex(position => position.Code)
            .IsUnique();
    }
}
