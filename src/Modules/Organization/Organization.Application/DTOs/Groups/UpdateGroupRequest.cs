namespace Organization.Application.DTOs.Groups;

public sealed record UpdateGroupRequest(
    string Code,
    string Name,
    Guid DepartmentId);