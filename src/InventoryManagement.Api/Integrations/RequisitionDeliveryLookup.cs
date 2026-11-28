using Delivery.Application.DTOs;
using Delivery.Application.Interfaces;
using Requisitions.Application.Interfaces;

namespace InventoryManagement.Api.Integrations;

public sealed class RequisitionDeliveryLookup : IRequisitionDeliveryLookup
{
    private readonly IRequisitionService _requisitionService;

    public RequisitionDeliveryLookup(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    public async Task<RequisitionDeliverySource?> GetRequisitionAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default)
    {
        var requisition = await _requisitionService.GetByIdAsync(
            requisitionId,
            cancellationToken);

        return requisition is null
            ? null
            : new RequisitionDeliverySource(
                requisition.Id,
                requisition.RequisitionNumber,
                requisition.Status);
    }
}
