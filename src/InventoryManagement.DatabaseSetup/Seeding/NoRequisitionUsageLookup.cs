using Inventory.Application.Interfaces;

namespace InventoryManagement.DatabaseSetup.Seeding;

// DatabaseSetup only migrates and seeds data; it never deactivates
// inventory items, so every item is reported as unused.
internal sealed class NoRequisitionUsageLookup : IInventoryItemUsageLookup
{
    public Task<bool> IsItemOnOpenRequisitionAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}
