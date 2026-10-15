namespace Organization.Application.DTOs.Departments;

public sealed record CreateDepartmentRequest(
    string Code,
    string Name);
