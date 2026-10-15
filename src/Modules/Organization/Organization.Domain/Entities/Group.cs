namespace Organization.Domain.Entities;

public sealed class Group
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid DepartmentId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Group()
    {
        // Required by EF Core.
    }

    public Group(string code, string name, Guid departmentId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Group code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.", nameof(name));

        if (departmentId == Guid.Empty)
            throw new ArgumentException("A valid department is required.", nameof(departmentId));

        Id = Guid.NewGuid();
        Code = code.Trim();
        Name = name.Trim();
        DepartmentId = departmentId;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(string code, string name, Guid departmentId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Group code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.", nameof(name));

        if (departmentId == Guid.Empty)
            throw new ArgumentException("A valid department is required.", nameof(departmentId));

        Code = code.Trim();
        Name = name.Trim();
        DepartmentId = departmentId;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}