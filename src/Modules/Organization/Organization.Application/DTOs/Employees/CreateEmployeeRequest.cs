
namespace Organization.Application.DTOs.Employees;

public sealed record CreateEmployeeRequest(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    Guid DepartmentId,
    Guid? GroupId,
    Guid? PositionId,
    string? MiddleName,
    string? Email,
    string? ContactNumber);
