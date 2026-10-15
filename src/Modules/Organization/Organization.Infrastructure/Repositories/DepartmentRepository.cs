using Microsoft.EntityFrameworkCore;
using Organization.Application.Contracts;
using Organization.Domain.Entities;
using Organization.Infrastructure.Persistence;

namespace Organization.Infrastructure.Repositories;

public sealed class DepartmentRepository(
    OrganizationDbContext dbContext) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .OrderBy(department => department.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Departments
            .FirstOrDefaultAsync(
                department => department.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Departments.AnyAsync(
            department =>
                department.Code == code &&
                (!excludeId.HasValue || department.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Department department,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Departments.AddAsync(
            department,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}