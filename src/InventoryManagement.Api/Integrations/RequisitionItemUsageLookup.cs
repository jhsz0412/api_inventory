using Inventory.Application.Interfaces;
using Requisitions.Application.Interfaces;

namespace InventoryManagement.Api.Integrations;

public sealed class RequisitionItemUsageLookup(
    IRequisitionRepository requisitionRepository)
    : IInventoryItemUsageLookup
{
    public Task<bool> IsItemOnOpenRequisitionAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        return requisitionRepository.IsInventoryItemOnOpenRequisitionAsync(
            inventoryItemId,
            cancellationToken);
    }
}
