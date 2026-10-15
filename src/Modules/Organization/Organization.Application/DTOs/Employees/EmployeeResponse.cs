
namespace Organization.Application.DTOs.Employees;

public sealed record EmployeeResponse(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string? MiddleName,
    string? Email,
    string? ContactNumber,
    Guid DepartmentId,
    Guid? GroupId,
    Guid? PositionId,
    bool IsActive,
    DateTime CreatedAtUtc);
