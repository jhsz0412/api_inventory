
using Organization.Domain.Entities;

namespace Organization.Application.Contracts;

public interface IGroupMemberRepository
{
    Task<bool> GroupExistsAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetMembersAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetEmployeesForMembershipAsync(
        IReadOnlyCollection<Guid> employeeIds,
        Guid? departmentId,
        bool includeAllEmployees,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetMemberEmployeeIdsAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task AddMembersAsync(
        Guid groupId,
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveMemberAsync(
        Guid groupId,
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
