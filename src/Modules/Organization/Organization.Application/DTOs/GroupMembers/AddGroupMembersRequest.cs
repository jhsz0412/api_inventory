
namespace Organization.Application.DTOs.GroupMembers;

public sealed record AddGroupMembersRequest(
    IReadOnlyCollection<Guid>? EmployeeIds,
    Guid? DepartmentId,
    bool IncludeAllEmployees = false);
