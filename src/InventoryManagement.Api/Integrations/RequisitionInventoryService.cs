using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Requisitions.Application.Interfaces;
using RequisitionStockIssueRequest = Requisitions.Application.DTOs.RequisitionStockIssueRequest;

namespace InventoryManagement.Api.Integrations;

public sealed class RequisitionInventoryService : IRequisitionInventoryService
{
    private readonly IInventoryService _inventoryService;

    public RequisitionInventoryService(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<decimal?> GetAvailableQuantityAsync(
        Guid inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        var item = await _inventoryService.GetItemByIdAsync(
            inventoryItemId,
            cancellationToken);

        if (item is null)
            return null;

        var balance = await _inventoryService.GetStockBalanceAsync(
            inventoryItemId,
            cancellationToken);

        return balance?.QuantityOnHand ?? 0m;
    }

    public Task IssueStockBatchAsync(
        IReadOnlyCollection<RequisitionStockIssueRequest> requests,
        string referenceNumber,
        CancellationToken cancellationToken = default)
    {
        var inventoryRequests = requests
            .Select(request => new IssueInventoryStockRequest(
                request.InventoryItemId,
                request.Quantity))
            .ToArray();

        return _inventoryService.IssueStockBatchAsync(
            inventoryRequests,
            referenceNumber,
            cancellationToken);
    }
}
