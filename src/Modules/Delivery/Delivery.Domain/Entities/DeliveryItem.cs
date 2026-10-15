
namespace Delivery.Domain.Entities;

public sealed class DeliveryItem
{
    public Guid Id { get; private set; }

    public Guid DeliveryId { get; private set; }

    public Guid InventoryItemId { get; private set; }

    public Guid StorageLocationId { get; private set; }

    public decimal QuantityPrepared { get; private set; }

    private DeliveryItem()
    {
    }

    public DeliveryItem(
        Guid inventoryItemId,
        Guid storageLocationId,
        decimal quantityPrepared)
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

        if (quantityPrepared <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantityPrepared),
                "Prepared quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        InventoryItemId = inventoryItemId;
        StorageLocationId = storageLocationId;
        QuantityPrepared = quantityPrepared;
    }
    internal void AssignToDelivery(Guid deliveryId)
    {
        if (deliveryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Delivery ID is required.",
                nameof(deliveryId));
        }

        if (DeliveryId != Guid.Empty && DeliveryId != deliveryId)
        {
            throw new InvalidOperationException(
                "A delivery item cannot be moved to another delivery.");
        }

        DeliveryId = deliveryId;
    }
}
