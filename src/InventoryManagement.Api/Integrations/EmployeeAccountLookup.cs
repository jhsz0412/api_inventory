using Identity.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Organization.Infrastructure.Persistence;

namespace InventoryManagement.Api.Integrations;

public sealed class EmployeeAccountLookup(
    OrganizationDbContext organizationDbContext)
    : IEmployeeAccountLookup
{
    public Task<bool> IsActiveEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return organizationDbContext.Employees
            .AnyAsync(
                employee => employee.Id == employeeId
                    && employee.IsActive,
                cancellationToken);
    }
}
