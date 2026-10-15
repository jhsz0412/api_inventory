using Microsoft.AspNetCore.Mvc;
using Reporting.Application.DTOs;
using Reporting.Application.Interfaces;
using Reporting.Domain.Entities;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;

namespace Reporting.Presentation.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportingController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportingController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("inventory-stock")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<InventoryStockReport>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<InventoryStockReport>>>
        GetInventoryStockReport(
            [FromQuery] InventoryStockReportRequest request,
            CancellationToken cancellationToken)
    {
        var report = await _reportingService.GetInventoryStockReportAsync(
            request,
            cancellationToken);

        return Ok(report);
    }

    [HttpGet("low-stock")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<LowStockReport>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<LowStockReport>>>
        GetLowStockReport(
            CancellationToken cancellationToken)
        {
            var report = await _reportingService.GetLowStockReportAsync(
                cancellationToken);

            return Ok(report);
        }

    [HttpGet("stock-movements")]
    [ProducesResponseType(
    typeof(IReadOnlyCollection<StockMovementReport>),
    StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockMovements(
    [FromQuery] Guid? inventoryItemId,
    [FromQuery] DateTime? fromUtc,
    [FromQuery] DateTime? toUtc,
    CancellationToken cancellationToken)
    {
        var results = await _reportingService.GetStockMovementReportAsync(
            inventoryItemId,
            fromUtc,
            toUtc,
            cancellationToken);

        return Ok(results);
    }

    [HttpGet("requisitions")]
    [ProducesResponseType(
    typeof(IReadOnlyCollection<RequisitionReport>),
    StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequisitions(
    [FromQuery] Guid? requestedByEmployeeId,
    [FromQuery] Guid? departmentId,
    [FromQuery] Guid? fiscalYearId,
    [FromQuery] string? status,
    [FromQuery] DateTime? fromUtc,
    [FromQuery] DateTime? toUtc,
    CancellationToken cancellationToken)
    {
        var request = new RequisitionReportRequest
        {
            RequestedByEmployeeId = requestedByEmployeeId,
            DepartmentId = departmentId,
            FiscalYearId = fiscalYearId,
            Status = status,
            FromUtc = fromUtc,
            ToUtc = toUtc
        };

        var results = await _reportingService.GetRequisitionReportAsync(
            request,
            cancellationToken);

        return Ok(results);
    }

    [HttpGet("deliveries")]
    public async Task<IActionResult> GetDeliveryReport(
        [FromQuery] DeliveryReportRequest request,
        CancellationToken cancellationToken)
    {
        var report = await _reportingService.GetDeliveryReportAsync(
            request,
            cancellationToken);

        return Ok(report);
    }

    [HttpGet("inventory-summary")]
    public async Task<IActionResult> GetInventoryOverviewSummary(
    CancellationToken cancellationToken)
    {
        var summary = await _reportingService
            .GetInventoryOverviewSummaryAsync(cancellationToken);

        return Ok(summary);
    }

    [HttpGet("requisition-summary")]
    public async Task<IActionResult> GetRequisitionStatusSummary(
    CancellationToken cancellationToken)
    {
        var summary = await _reportingService
            .GetRequisitionStatusSummaryAsync(cancellationToken);

        return Ok(summary);
    }
}