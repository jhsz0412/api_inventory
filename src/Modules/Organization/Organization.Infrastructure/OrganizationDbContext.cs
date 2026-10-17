using Microsoft.EntityFrameworkCore;
using Organization.Domain.Entities;
using Organization.Infrastructure.Persistence.Configurations;

namespace Organization.Infrastructure.Persistence;

public sealed class OrganizationDbContext(
    DbContextOptions<OrganizationDbContext> options)
    : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrganizationDbContext).Assembly);

    }
}