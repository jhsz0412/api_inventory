namespace Inventory.Domain.Entities;

public sealed class InventoryStockBalance
{
    public Guid Id { get; private set; }

    public Guid InventoryItemId { get; private set; }

    public decimal QuantityOnHand { get; private set; }

    public DateTime LastUpdatedUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    private InventoryStockBalance()
    {
    }

    public InventoryStockBalance(Guid inventoryItemId)
    {
        if (inventoryItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory item ID is required.",
                nameof(inventoryItemId));
        }

        Id = Guid.NewGuid();
        InventoryItemId = inventoryItemId;
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
                "Insufficient stock available.");
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