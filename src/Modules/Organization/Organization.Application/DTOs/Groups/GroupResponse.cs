namespace Organization.Application.DTOs.Groups;

public sealed record GroupResponse(
    Guid Id,
    string Code,
    string Name,
    Guid DepartmentId,
    bool IsActive,
    DateTime CreatedAtUtc);