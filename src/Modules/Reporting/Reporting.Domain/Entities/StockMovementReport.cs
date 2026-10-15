namespace Reporting.Domain.Entities;

public sealed class StockMovementReport
{
    public Guid TransactionId { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public string ItemCode { get; private set; }
    public string ItemName { get; private set; }
    public string TransactionType { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal QuantityBefore { get; private set; }
    public decimal QuantityAfter { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? Remarks { get; private set; }
    public Guid? StorageLocationId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public StockMovementReport(
        Guid transactionId,
        Guid inventoryItemId,
        string itemCode,
        string itemName,
        string transactionType,
        decimal quantity,
        decimal quantityBefore,
        decimal quantityAfter,
        string? referenceNumber,
        string? remarks,
        Guid? storageLocationId,
        DateTime createdAtUtc)
    {
        TransactionId = transactionId;
        InventoryItemId = inventoryItemId;
        ItemCode = itemCode;
        ItemName = itemName;
        TransactionType = transactionType;
        Quantity = quantity;
        QuantityBefore = quantityBefore;
        QuantityAfter = quantityAfter;
        ReferenceNumber = referenceNumber;
        Remarks = remarks;
        StorageLocationId = storageLocationId;
        CreatedAtUtc = createdAtUtc;
    }
}
