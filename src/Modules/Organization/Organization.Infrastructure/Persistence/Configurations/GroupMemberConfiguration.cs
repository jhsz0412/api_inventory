
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Persistence.Configurations;

public sealed class GroupMemberConfiguration
    : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> entity)
    {
        entity.ToTable("GroupMembers", "Organization");

        entity.HasKey(member => member.Id);

        entity.Property(member => member.GroupId)
            .IsRequired();

        entity.Property(member => member.EmployeeId)
            .IsRequired();

        entity.Property(member => member.CreatedAtUtc)
            .IsRequired();

        entity.HasOne<Group>()
            .WithMany()
            .HasForeignKey(member => member.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(member => member.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(member => new
        {
            member.GroupId,
            member.EmployeeId
        })
            .IsUnique();

        entity.HasIndex(member => member.EmployeeId);
    }
}
