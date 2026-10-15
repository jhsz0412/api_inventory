namespace Inventory.Domain.Entities;

public sealed class InventoryLocationStockBalance
{
    public Guid Id { get; private set; }

    public Guid InventoryItemId { get; private set; }

    public Guid StorageLocationId { get; private set; }

    public decimal QuantityOnHand { get; private set; }

    public DateTime LastUpdatedUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    private InventoryLocationStockBalance()
    {
    }

    public InventoryLocationStockBalance(
        Guid inventoryItemId,
        Guid storageLocationId)
    {
        if (inventoryItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory item ID is required.",
                nameof(inventoryItemId));
        }

        if (storageLocationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Storage location ID is required.",
                nameof(storageLocationId));
        }

        Id = Guid.NewGuid();
        InventoryItemId = inventoryItemId;
        StorageLocationId = storageLocationId;
        QuantityOnHand = 0;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    public void IncreaseStock(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        QuantityOnHand += quantity;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    public void DecreaseStock(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        if (quantity > QuantityOnHand)
        {
            throw new InvalidOperationException(
                "Insufficient stock available at this location.");
        }

        QuantityOnHand -= quantity;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    public void AdjustStock(decimal newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(newQuantity),
                "Stock quantity cannot be negative.");
        }

        QuantityOnHand = newQuantity;
        LastUpdatedUtc = DateTime.UtcNow;
    }
}