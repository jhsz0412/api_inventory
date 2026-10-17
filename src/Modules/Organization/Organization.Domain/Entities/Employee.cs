
namespace Organization.Domain.Entities;

public sealed class Employee
{
    public Guid Id { get; private set; }

    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string? Email { get; private set; }
    public string? ContactNumber { get; private set; }

    public Guid DepartmentId { get; private set; }
    public Guid? PositionId { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Employee() { }

    public Employee(
        string employeeNumber,
        string firstName,
        string lastName,
        Guid departmentId,
        Guid? positionId = null,
        string? middleName = null,
        string? email = null,
        string? contactNumber = null)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException(
                "Employee number is required.", nameof(employeeNumber));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException(
                "First name is required.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException(
                "Last name is required.", nameof(lastName));

        if (departmentId == Guid.Empty)
            throw new ArgumentException(
                "Department is required.", nameof(departmentId));

        Id = Guid.NewGuid();
        EmployeeNumber = employeeNumber.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = NormalizeOptional(middleName);
        Email = NormalizeOptional(email);
        ContactNumber = NormalizeOptional(contactNumber);

        DepartmentId = departmentId;
        PositionId = positionId;

        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        Guid departmentId,
        Guid? positionId,
        string? middleName,
        string? email,
        string? contactNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException(
                "First name is required.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException(
                "Last name is required.", nameof(lastName));

        if (departmentId == Guid.Empty)
            throw new ArgumentException(
                "Department is required.", nameof(departmentId));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = NormalizeOptional(middleName);
        Email = NormalizeOptional(email);
        ContactNumber = NormalizeOptional(contactNumber);

        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
