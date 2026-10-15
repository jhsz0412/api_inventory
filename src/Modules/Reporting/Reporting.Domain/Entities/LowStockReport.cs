namespace Reporting.Domain.Entities;

public sealed class LowStockReport
{
    public Guid InventoryItemId { get; private set; }
    public string ItemCode { get; private set; }
    public string ItemName { get; private set; }
    public decimal QuantityOnHand { get; private set; }
    public decimal ReorderLevel { get; private set; }
    public decimal QuantityToReorder { get; private set; }
    public DateTime LastUpdatedUtc { get; private set; }

    public LowStockReport(
        Guid inventoryItemId,
        string itemCode,
        string itemName,
        decimal quantityOnHand,
        decimal reorderLevel,
        DateTime lastUpdatedUtc)
    {
        if (inventoryItemId == Guid.Empty)
            throw new ArgumentException("Inventory item ID is required.");

        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code is required.");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name is required.");

        if (quantityOnHand < 0)
            throw new ArgumentOutOfRangeException(nameof(quantityOnHand));

        if (reorderLevel < 0)
            throw new ArgumentOutOfRangeException(nameof(reorderLevel));

        InventoryItemId = inventoryItemId;
        ItemCode = itemCode.Trim();
        ItemName = itemName.Trim();
        QuantityOnHand = quantityOnHand;
        ReorderLevel = reorderLevel;
        QuantityToReorder = Math.Max(0, reorderLevel - quantityOnHand);
        LastUpdatedUtc = lastUpdatedUtc;
    }
}