
using Organization.Application.Contracts;
using Organization.Application.DTOs.GroupMembers;

namespace Organization.Application.Services.GroupMembers;

public sealed class GroupMemberService(
    IGroupMemberRepository repository) : IGroupMemberService
{
    public async Task<IReadOnlyList<GroupMemberResponse>> GetMembersAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGroupExistsAsync(groupId, cancellationToken);

        var employees = await repository.GetMembersAsync(
            groupId, cancellationToken);

        return employees.Select(employee => new GroupMemberResponse(
            employee.Id,
            employee.EmployeeNumber,
            employee.FirstName,
            employee.LastName,
            employee.DepartmentId)).ToList();
    }

    public async Task<int> AddMembersAsync(
        Guid groupId,
        AddGroupMembersRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureGroupExistsAsync(groupId, cancellationToken);

        var employeeIds = request.EmployeeIds ?? Array.Empty<Guid>();

        var selectionModes =
            (request.IncludeAllEmployees ? 1 : 0) +
            (request.DepartmentId.HasValue ? 1 : 0) +
            (employeeIds.Count > 0 ? 1 : 0);

        if (selectionModes != 1)
        {
            throw new ArgumentException(
                "Choose exactly one selection method: employee IDs, " +
                "department ID, or IncludeAllEmployees.");
        }

        var employees = await repository.GetEmployeesForMembershipAsync(
            employeeIds,
            request.DepartmentId,
            request.IncludeAllEmployees,
            cancellationToken);

        if (!request.IncludeAllEmployees &&
            !request.DepartmentId.HasValue &&
            employees.Count != employeeIds.Distinct().Count())
        {
            throw new ArgumentException(
                "One or more selected employees do not exist or are inactive.");
        }

        if (request.DepartmentId.HasValue && employees.Count == 0)
        {
            // An empty department is valid; it simply adds no members.
            return 0;
        }

        var ids = employees.Select(employee => employee.Id).ToArray();

        await repository.AddMembersAsync(
            groupId, ids, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return ids.Length;
    }

    public async Task<int> CopyMembersAsync(
        Guid groupId,
        Guid sourceGroupId,
        CancellationToken cancellationToken = default)
    {
        if (groupId == sourceGroupId)
            throw new ArgumentException(
                "A group cannot copy members from itself.");

        await EnsureGroupExistsAsync(groupId, cancellationToken);
        await EnsureGroupExistsAsync(sourceGroupId, cancellationToken);

        var sourceIds = await repository.GetMemberEmployeeIdsAsync(
            sourceGroupId, cancellationToken);

        if (sourceIds.Count == 0)
            return 0;

        await repository.AddMembersAsync(
            groupId, sourceIds, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return sourceIds.Count;
    }

    public async Task<bool> RemoveMemberAsync(
        Guid groupId,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        await EnsureGroupExistsAsync(groupId, cancellationToken);

        var removed = await repository.RemoveMemberAsync(
            groupId, employeeId, cancellationToken);

        if (removed)
            await repository.SaveChangesAsync(cancellationToken);

        return removed;
    }

    private async Task EnsureGroupExistsAsync(
        Guid groupId,
        CancellationToken cancellationToken)
    {
        if (!await repository.GroupExistsAsync(
                groupId, cancellationToken))
        {
            throw new KeyNotFoundException(
                $"Group '{groupId}' was not found.");
        }
    }
}
