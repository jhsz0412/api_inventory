
namespace Inventory.Domain.Entities;

public sealed class InventoryCategory
{
    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    private InventoryCategory()
    {
    }

    public InventoryCategory(
        string code,
        string name,
        string? description = null)
    {
        Id = Guid.NewGuid();

        SetCode(code);
        SetName(name);
        SetDescription(description);

        IsActive = true;
    }

    public void Update(
        string name,
        string? description)
    {
        SetName(name);
        SetDescription(description);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Category code is required.", nameof(code));

        code = code.Trim().ToUpperInvariant();

        if (code.Length > 30)
            throw new ArgumentException(
                "Category code cannot exceed 30 characters.",
                nameof(code));

        Code = code;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Category name is required.", nameof(name));

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException(
                "Category name cannot exceed 100 characters.",
                nameof(name));

        Name = name;
    }

    private void SetDescription(string? description)
    {
        description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        if (description?.Length > 500)
            throw new ArgumentException(
                "Description cannot exceed 500 characters.",
                nameof(description));

        Description = description;
    }
}
