using Delivery.Application.DTOs;

namespace Delivery.Application.Interfaces;

public interface IRequisitionDeliveryLookup
{
    Task<RequisitionDeliverySource?> GetRequisitionAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);
}
