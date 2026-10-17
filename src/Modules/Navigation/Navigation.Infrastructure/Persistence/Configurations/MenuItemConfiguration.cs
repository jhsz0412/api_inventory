
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigation.Domain.Entities;

namespace Navigation.Infrastructure.Persistence.Configurations;

public sealed class MenuItemConfiguration
    : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");

        builder.HasKey(menu => menu.Id);

        builder.Property(menu => menu.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(menu => menu.Label)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(menu => menu.Route)
            .HasMaxLength(300);

        builder.Property(menu => menu.Icon)
            .HasMaxLength(100);

        builder.Property(menu => menu.SortOrder)
            .IsRequired();

        builder.Property(menu => menu.IsActive)
            .IsRequired();

        builder.Property(menu => menu.CreatedAtUtc)
            .IsRequired();

        builder.Property(menu => menu.UpdatedAtUtc);

        builder.HasIndex(menu => menu.Name)
            .IsUnique();

        builder.HasIndex(menu => menu.ParentMenuItemId);

        builder.HasIndex(menu => new
        {
            menu.ParentMenuItemId,
            menu.SortOrder
        });

        builder.HasOne(menu => menu.ParentMenuItem)
            .WithMany(menu => menu.Children)
            .HasForeignKey(menu => menu.ParentMenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(menu => menu.ParentMenuItem)
        .WithMany(menu => menu.Children)
        .HasForeignKey(menu => menu.ParentMenuItemId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
