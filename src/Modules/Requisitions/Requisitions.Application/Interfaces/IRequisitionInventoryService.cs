
using Requisitions.Application.DTOs;

namespace Requisitions.Application.Interfaces;

public interface IRequisitionInventoryService
{
    Task<decimal?> GetAvailableQuantityAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default);

    Task IssueStockBatchAsync(
        IReadOnlyCollection<RequisitionStockIssueRequest> requests,
        string referenceNumber,
        CancellationToken cancellationToken = default);
}
