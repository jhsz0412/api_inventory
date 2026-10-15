
using Microsoft.EntityFrameworkCore;
using Organization.Application.Contracts;
using Organization.Domain.Entities;
using Organization.Infrastructure.Persistence;

namespace Organization.Infrastructure.Repositories;

public sealed class EmployeeRepository(
    OrganizationDbContext dbContext) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.EmployeeNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .FirstOrDefaultAsync(
                employee => employee.Id == id,
                cancellationToken);
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .FirstOrDefaultAsync(
                employee => employee.EmployeeNumber == employeeNumber,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmployeeNumberAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees.AnyAsync(
            employee => employee.EmployeeNumber == employeeNumber,
            cancellationToken);
    }

    public async Task AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Employees.AddAsync(
            employee,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
