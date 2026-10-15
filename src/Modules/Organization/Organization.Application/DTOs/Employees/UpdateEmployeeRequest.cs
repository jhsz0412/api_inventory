
namespace Organization.Application.DTOs.Employees;

public sealed record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    Guid DepartmentId,
    Guid? GroupId,
    Guid? PositionId,
    string? MiddleName,
    string? Email,
    string? ContactNumber);
