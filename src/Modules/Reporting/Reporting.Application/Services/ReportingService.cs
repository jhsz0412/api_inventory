using Reporting.Application.DTOs;
using Reporting.Application.Interfaces;
using Reporting.Domain.Entities;

namespace Reporting.Application.Services;

public sealed class ReportingService : IReportingService
{
    private readonly IReportingRepository _repository;

    public ReportingService(IReportingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<InventoryStockReport>>
        GetInventoryStockReportAsync(
            InventoryStockReportRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _repository.GetInventoryStockReportAsync(
            request.InventoryItemId,
            request.WarehouseId,
            request.StorageLocationId,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<LowStockReport>>
        GetLowStockReportAsync(
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetLowStockReportAsync(
                cancellationToken);
        }

    public async Task<IReadOnlyCollection<StockMovementReport>>
    GetStockMovementReportAsync(
        Guid? inventoryItemId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        if (fromUtc.HasValue &&
            toUtc.HasValue &&
            fromUtc.Value > toUtc.Value)
        {
            throw new ArgumentException(
                "The start date must not be later than the end date.");
        }

        return await _repository.GetStockMovementReportAsync(
            inventoryItemId,
            fromUtc,
            toUtc,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<RequisitionReport>>
    GetRequisitionReportAsync(
        RequisitionReportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _repository.GetRequisitionReportAsync(
            request,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<DeliveryReport>> GetDeliveryReportAsync(
    DeliveryReportRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _repository.GetDeliveryReportAsync(
            request,
            cancellationToken);
    }

    public async Task<InventoryOverviewSummary>
    GetInventoryOverviewSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetInventoryOverviewSummaryAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<RequisitionStatusSummary>>
    GetRequisitionStatusSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetRequisitionStatusSummaryAsync(
            cancellationToken);
    }
}