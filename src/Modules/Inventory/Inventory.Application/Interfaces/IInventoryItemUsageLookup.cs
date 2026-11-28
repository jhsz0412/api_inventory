namespace Inventory.Application.Interfaces;

public interface IInventoryItemUsageLookup
{
    Task<bool> IsItemOnOpenRequisitionAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default);
}
