
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Navigation.Domain.Entities;

namespace Navigation.Infrastructure.Persistence.Configurations;

public sealed class GroupMenuItemConfiguration
    : IEntityTypeConfiguration<GroupMenuItem>
{
    public void Configure(EntityTypeBuilder<GroupMenuItem> builder)
    {
        builder.ToTable("GroupMenuItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.GroupId)
            .IsRequired();

        builder.Property(item => item.MenuItemId)
            .IsRequired();

        builder.Property(item => item.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(item => new
        {
            item.GroupId,
            item.MenuItemId
        }).IsUnique();

        builder.HasIndex(item => item.MenuItemId);

        builder.HasOne<MenuItem>()
            .WithMany()
            .HasForeignKey(item => item.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
