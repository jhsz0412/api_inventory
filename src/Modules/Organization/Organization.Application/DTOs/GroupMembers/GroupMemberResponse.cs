
namespace Organization.Application.DTOs.GroupMembers;

public sealed record GroupMemberResponse(
    Guid EmployeeId,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    Guid DepartmentId);
