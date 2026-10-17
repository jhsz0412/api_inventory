
using Organization.Application.Contracts;
using Organization.Application.DTOs.Employees;
using Organization.Domain.Entities;

namespace Organization.Application.Services.Employees;

public sealed class EmployeeService(
    IEmployeeRepository employeeRepository,
    IDepartmentRepository departmentRepository,
    IPositionRepository positionRepository) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(
            cancellationToken);

        return employees.Select(MapToResponse).ToList();
    }

    public async Task<EmployeeResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(
            id, cancellationToken);

        return employee is null ? null : MapToResponse(employee);
    }

    public async Task<EmployeeResponse> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var employeeNumber = request.EmployeeNumber?.Trim();

        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException(
                "Employee number is required.");

        if (await employeeRepository.ExistsByEmployeeNumberAsync(
                employeeNumber, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Employee number '{employeeNumber}' already exists.");
        }

        await ValidateOrganizationReferencesAsync(
            request.DepartmentId,
            request.PositionId,
            cancellationToken);

        var employee = new Employee(
            employeeNumber,
            request.FirstName,
            request.LastName,
            request.DepartmentId,
            request.PositionId,
            request.MiddleName,
            request.Email,
            request.ContactNumber);

        await employeeRepository.AddAsync(
            employee, cancellationToken);

        await employeeRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(employee);
    }

    public async Task<EmployeeResponse?> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(
            id, cancellationToken);

        if (employee is null)
            return null;

        await ValidateOrganizationReferencesAsync(
            request.DepartmentId,
            request.PositionId,
            cancellationToken);

        employee.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.DepartmentId,
            request.PositionId,
            request.MiddleName,
            request.Email,
            request.ContactNumber);

        await employeeRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(employee);
    }

    public async Task<bool> SetStatusAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(
            id, cancellationToken);

        if (employee is null)
            return false;

        if (isActive)
            employee.Activate();
        else
            employee.Deactivate();

        await employeeRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private async Task ValidateOrganizationReferencesAsync(
        Guid departmentId,
        Guid? positionId,
        CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetByIdAsync(
            departmentId, cancellationToken);

        if (department is null)
            throw new ArgumentException(
                "The selected department does not exist.");

        if (!department.IsActive)
            throw new InvalidOperationException(
                "The selected department is inactive.");

        if (positionId.HasValue)
        {
            var position = await positionRepository.GetByIdAsync(
                positionId.Value, cancellationToken);

            if (position is null)
                throw new ArgumentException(
                    "The selected position does not exist.");

            if (!position.IsActive)
                throw new InvalidOperationException(
                    "The selected position is inactive.");
        }
    }

    private static EmployeeResponse MapToResponse(Employee employee) =>
        new(
            employee.Id,
            employee.EmployeeNumber,
            employee.FirstName,
            employee.LastName,
            employee.MiddleName,
            employee.Email,
            employee.ContactNumber,
            employee.DepartmentId,
            employee.PositionId,
            employee.IsActive,
            employee.CreatedAtUtc);
}
