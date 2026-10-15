using Microsoft.EntityFrameworkCore;
using Organization.Application.Contracts;
using Organization.Domain.Entities;
using Organization.Infrastructure.Persistence;

namespace Organization.Infrastructure.Repositories;

public sealed class GroupRepository(
    OrganizationDbContext dbContext) : IGroupRepository
{
    public async Task<IReadOnlyList<Group>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Groups
            .AsNoTracking()
            .OrderBy(group => group.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Group?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Groups
            .FirstOrDefaultAsync(
                group => group.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid departmentId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Groups.AnyAsync(
            group =>
                group.Code == code &&
                group.DepartmentId == departmentId &&
                (!excludeId.HasValue || group.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<bool> DepartmentExistsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Departments.AnyAsync(
            department => department.Id == departmentId,
            cancellationToken);
    }

    public async Task AddAsync(
        Group group,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Groups.AddAsync(group, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}