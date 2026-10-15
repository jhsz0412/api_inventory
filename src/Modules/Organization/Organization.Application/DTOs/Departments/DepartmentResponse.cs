namespace Organization.Application.DTOs.Departments;

public sealed record DepartmentResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);
