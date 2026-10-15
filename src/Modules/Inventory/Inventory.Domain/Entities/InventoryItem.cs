
namespace Inventory.Domain.Entities;

public sealed class InventoryItem
{
    public Guid Id { get; private set; }

    public string ItemCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid CategoryId { get; private set; }

    public Guid UnitOfMeasureId { get; private set; }

    public decimal ReorderLevel { get; private set; }

    public bool IsActive { get; private set; }

    private InventoryItem()
    {
    }

    public InventoryItem(
        string itemCode,
        string name,
        Guid categoryId,
        Guid unitOfMeasureId,
        string? description = null,
        decimal reorderLevel = 0)
    {
        Id = Guid.NewGuid();

        SetItemCode(itemCode);
        SetName(name);
        SetDescription(description);
        SetCategory(categoryId);
        SetUnitOfMeasure(unitOfMeasureId);
        SetReorderLevel(reorderLevel);

        IsActive = true;
    }

    public void Update(
        string name,
        string? description,
        Guid categoryId,
        Guid unitOfMeasureId,
        decimal reorderLevel)
    {
        SetName(name);
        SetDescription(description);
        SetCategory(categoryId);
        SetUnitOfMeasure(unitOfMeasureId);
        SetReorderLevel(reorderLevel);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetItemCode(string itemCode)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException(
                "Item code is required.", nameof(itemCode));

        itemCode = itemCode.Trim().ToUpperInvariant();

        if (itemCode.Length > 50)
            throw new ArgumentException(
                "Item code cannot exceed 50 characters.",
                nameof(itemCode));

        ItemCode = itemCode;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Item name is required.", nameof(name));

        name = name.Trim();

        if (name.Length > 200)
            throw new ArgumentException(
                "Item name cannot exceed 200 characters.",
                nameof(name));

        Name = name;
    }

    private void SetDescription(string? description)
    {
        description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        if (description?.Length > 1000)
            throw new ArgumentException(
                "Description cannot exceed 1000 characters.",
                nameof(description));

        Description = description;
    }

    private void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException(
                "Category is required.", nameof(categoryId));

        CategoryId = categoryId;
    }

    private void SetUnitOfMeasure(Guid unitOfMeasureId)
    {
        if (unitOfMeasureId == Guid.Empty)
            throw new ArgumentException(
                "Unit of measure is required.",
                nameof(unitOfMeasureId));

        UnitOfMeasureId = unitOfMeasureId;
    }

    private void SetReorderLevel(decimal reorderLevel)
    {
        if (reorderLevel < 0)
            throw new ArgumentOutOfRangeException(
                nameof(reorderLevel),
                "Reorder level cannot be negative.");

        ReorderLevel = reorderLevel;
    }
}
