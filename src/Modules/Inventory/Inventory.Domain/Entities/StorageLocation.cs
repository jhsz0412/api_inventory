namespace Inventory.Domain.Entities;

public sealed class StorageLocation
{
    public Guid Id { get; private set; }

    public Guid WarehouseId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private StorageLocation()
    {
    }

    public StorageLocation(
        Guid warehouseId,
        string code,
        string name,
        string? description = null)
    {
        if (warehouseId == Guid.Empty)
        {
            throw new ArgumentException(
                "Warehouse ID is required.",
                nameof(warehouseId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Storage location code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Storage location name is required.",
                nameof(name));
        }

        Id = Guid.NewGuid();
        WarehouseId = warehouseId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Storage location name is required.",
                nameof(name));
        }

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}