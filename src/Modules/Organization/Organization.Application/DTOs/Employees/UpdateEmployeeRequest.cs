
namespace Organization.Application.DTOs.Employees;

public sealed record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    Guid DepartmentId,
    Guid? PositionId,
    string? MiddleName,
    string? Email,
    string? ContactNumber);
