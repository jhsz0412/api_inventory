namespace Organization.Application.DTOs.Departments;

public sealed record UpdateDepartmentRequest(
    string Code,
    string Name);
