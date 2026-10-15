
namespace Inventory.Domain.Entities;

public sealed class UnitOfMeasure
{
    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    private UnitOfMeasure()
    {
    }

    public UnitOfMeasure(string code, string name)
    {
        Id = Guid.NewGuid();

        SetCode(code);
        SetName(name);

        IsActive = true;
    }

    public void Update(string name)
    {
        SetName(name);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Unit code is required.", nameof(code));

        code = code.Trim().ToUpperInvariant();

        if (code.Length > 20)
            throw new ArgumentException(
                "Unit code cannot exceed 20 characters.",
                nameof(code));

        Code = code;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Unit name is required.", nameof(name));

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException(
                "Unit name cannot exceed 100 characters.",
                nameof(name));

        Name = name;
    }
}
