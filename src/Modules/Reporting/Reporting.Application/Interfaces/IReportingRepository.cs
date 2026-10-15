using Reporting.Application.DTOs;
using Reporting.Domain.Entities;

namespace Reporting.Application.Interfaces;

public interface IReportingRepository
{
    Task<IReadOnlyCollection<InventoryStockReport>>
        GetInventoryStockReportAsync(
            Guid? inventoryItemId = null,
            Guid? warehouseId = null,
            Guid? storageLocationId = null,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LowStockReport>>
        GetLowStockReportAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StockMovementReport>>
        GetStockMovementReportAsync(
            Guid? inventoryItemId = null,
            DateTime? fromUtc = null,
            DateTime? toUtc = null,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RequisitionReport>>
    GetRequisitionReportAsync(
        RequisitionReportRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DeliveryReport>> GetDeliveryReportAsync(
        DeliveryReportRequest request,
        CancellationToken cancellationToken = default);

    Task<InventoryOverviewSummary> GetInventoryOverviewSummaryAsync(
    CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RequisitionStatusSummary>>
    GetRequisitionStatusSummaryAsync(
        CancellationToken cancellationToken = default);
}