namespace Requisitions.Domain.Entities;

public sealed class RequisitionItem
{
    public Guid Id { get; private set; }

    public Guid RequisitionId { get; private set; }

    public Guid InventoryItemId { get; private set; }

    public int QuantityRequested { get; private set; }

    public string? Notes { get; private set; }

    private RequisitionItem()
    {
    }

    public RequisitionItem(
        Guid requisitionId,
        Guid inventoryItemId,
        int quantityRequested,
        string? notes = null)
    {
        if (requisitionId == Guid.Empty)
            throw new ArgumentException(
                "Requisition is required.", nameof(requisitionId));

        if (inventoryItemId == Guid.Empty)
            throw new ArgumentException(
                "Inventory item is required.", nameof(inventoryItemId));

        if (quantityRequested <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantityRequested),
                "Requested quantity must be greater than zero.");

        if (notes?.Trim().Length > 500)
            throw new ArgumentException(
                "Item notes cannot exceed 500 characters.",
                nameof(notes));

        Id = Guid.NewGuid();
        RequisitionId = requisitionId;
        InventoryItemId = inventoryItemId;
        QuantityRequested = quantityRequested;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public void ChangeQuantity(int quantityRequested)
    {
        if (quantityRequested <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantityRequested),
                "Requested quantity must be greater than zero.");

        QuantityRequested = quantityRequested;
    }

    public void UpdateNotes(string? notes)
    {
        if (notes?.Trim().Length > 500)
            throw new ArgumentException(
                "Item notes cannot exceed 500 characters.",
                nameof(notes));

        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}