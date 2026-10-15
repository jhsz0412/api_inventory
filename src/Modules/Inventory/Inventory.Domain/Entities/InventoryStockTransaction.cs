namespace Inventory.Domain.Entities;

public sealed class InventoryStockTransaction
{
    public Guid Id { get; private set; }

    public Guid InventoryItemId { get; private set; }

    public InventoryStockTransactionType TransactionType
    {
        get;
        private set;
    }

    public decimal Quantity { get; private set; }

    public decimal QuantityBefore { get; private set; }

    public decimal QuantityAfter { get; private set; }

    public string? ReferenceNumber { get; private set; }

    public string? Remarks { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public Guid? StorageLocationId { get; private set; }

    private InventoryStockTransaction()
    {
    }

    public InventoryStockTransaction(
        Guid inventoryItemId,
        InventoryStockTransactionType transactionType,
        decimal quantity,
        decimal quantityBefore,
        decimal quantityAfter,
        string? referenceNumber = null,
        string? remarks = null)
    {
        if (inventoryItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "Inventory item ID is required.",
                nameof(inventoryItemId));
        }

        if (!Enum.IsDefined(transactionType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(transactionType));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Transaction quantity must be greater than zero.");
        }

        if (quantityBefore < 0 || quantityAfter < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantityAfter),
                "Stock quantities cannot be negative.");
        }

        Id = Guid.NewGuid();
        InventoryItemId = inventoryItemId;
        TransactionType = transactionType;
        Quantity = quantity;
        QuantityBefore = quantityBefore;
        QuantityAfter = quantityAfter;
        ReferenceNumber = Normalize(referenceNumber, 100);
        Remarks = Normalize(remarks, 500);
        CreatedAtUtc = DateTime.UtcNow;
    }

    private static string? Normalize(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException(
                $"Value cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    public void AssignStorageLocation(Guid storageLocationId)
    {
        if (storageLocationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Storage location ID is required.",
                nameof(storageLocationId));
        }

        StorageLocationId = storageLocationId;
    }
}