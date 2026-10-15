namespace Reporting.Domain.Entities;

public sealed class InventoryStockReport
{
    public Guid InventoryItemId { get; private set; }

    public string ItemCode { get; private set; }

    public string ItemName { get; private set; }

    public Guid WarehouseId { get; private set; }

    public string WarehouseName { get; private set; }

    public Guid StorageLocationId { get; private set; }

    public string StorageLocationCode { get; private set; }

    public decimal QuantityOnHand { get; private set; }

    public DateTime LastUpdatedUtc { get; private set; }

    public InventoryStockReport(
        Guid inventoryItemId,
        string itemCode,
        string itemName,
        Guid warehouseId,
        string warehouseName,
        Guid storageLocationId,
        string storageLocationCode,
        decimal quantityOnHand,
        DateTime lastUpdatedUtc)
    {
        if (inventoryItemId == Guid.Empty)
            throw new ArgumentException("Inventory item ID is required.");

        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code is required.");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name is required.");

        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse ID is required.");

        if (string.IsNullOrWhiteSpace(warehouseName))
            throw new ArgumentException("Warehouse name is required.");

        if (storageLocationId == Guid.Empty)
            throw new ArgumentException("Storage location ID is required.");

        if (string.IsNullOrWhiteSpace(storageLocationCode))
            throw new ArgumentException("Storage location code is required.");

        if (quantityOnHand < 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantityOnHand),
                "Quantity on hand cannot be negative.");

        InventoryItemId = inventoryItemId;
        ItemCode = itemCode.Trim();
        ItemName = itemName.Trim();
        WarehouseId = warehouseId;
        WarehouseName = warehouseName.Trim();
        StorageLocationId = storageLocationId;
        StorageLocationCode = storageLocationCode.Trim();
        QuantityOnHand = quantityOnHand;
        LastUpdatedUtc = lastUpdatedUtc;
    }
}