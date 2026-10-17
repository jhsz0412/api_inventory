
using Microsoft.EntityFrameworkCore;
using Organization.Application.Contracts;
using Organization.Domain.Entities;
using Organization.Infrastructure.Persistence;

namespace Organization.Infrastructure.Repositories;

public sealed class GroupMemberRepository(
    OrganizationDbContext dbContext) : IGroupMemberRepository
{
    public Task<bool> GroupExistsAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Groups.AnyAsync(
            group => group.Id == groupId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetMembersAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from member in dbContext.GroupMembers.AsNoTracking()
            join employee in dbContext.Employees.AsNoTracking()
                on member.EmployeeId equals employee.Id
            where member.GroupId == groupId
            orderby employee.EmployeeNumber
            select employee
        ).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesForMembershipAsync(
        IReadOnlyCollection<Guid> employeeIds,
        Guid? departmentId,
        bool includeAllEmployees,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Employees
            .AsNoTracking()
            .Where(employee => employee.IsActive);

        if (includeAllEmployees)
        {
            // Include all active employees.
        }
        else if (departmentId.HasValue)
        {
            query = query.Where(
                employee => employee.DepartmentId == departmentId.Value);
        }
        else
        {
            query = query.Where(
                employee => employeeIds.Contains(employee.Id));
        }

        return await query
            .OrderBy(employee => employee.EmployeeNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetMemberEmployeeIdsAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.GroupMembers
            .AsNoTracking()
            .Where(member => member.GroupId == groupId)
            .Select(member => member.EmployeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMembersAsync(
        Guid groupId,
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken cancellationToken = default)
    {
        var existingIds = await dbContext.GroupMembers
            .Where(member =>
                member.GroupId == groupId &&
                employeeIds.Contains(member.EmployeeId))
            .Select(member => member.EmployeeId)
            .ToListAsync(cancellationToken);

        var existingSet = existingIds.ToHashSet();

        var newMembers = employeeIds
            .Distinct()
            .Where(employeeId => !existingSet.Contains(employeeId))
            .Select(employeeId => new GroupMember(groupId, employeeId))
            .ToList();

        await dbContext.GroupMembers.AddRangeAsync(
            newMembers, cancellationToken);
    }

    public async Task<bool> RemoveMemberAsync(
        Guid groupId,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var member = await dbContext.GroupMembers
            .FirstOrDefaultAsync(
                item => item.GroupId == groupId &&
                        item.EmployeeId == employeeId,
                cancellationToken);

        if (member is null)
            return false;

        dbContext.GroupMembers.Remove(member);
        return true;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
