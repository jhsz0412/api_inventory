namespace Requisitions.Application.DTOs;

public sealed record RequisitionResponse(
    Guid Id,
    string RequisitionNumber,
    Guid RequestedByEmployeeId,
    Guid DepartmentId,
    Guid FiscalYearId,
    DateTime RequestedAtUtc,
    string Purpose,
    string Status,
    IReadOnlyCollection<RequisitionItemResponse> Items);

public sealed record RequisitionItemResponse(
    Guid Id,
    Guid InventoryItemId,
    int QuantityRequested,
    string? Notes);