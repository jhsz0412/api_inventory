namespace Reporting.Domain.Entities;

public sealed class RequisitionReport
{
    public Guid RequisitionId { get; private set; }

    public string RequisitionNumber { get; private set; }

    public Guid RequestedByEmployeeId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid FiscalYearId { get; private set; }

    public DateTime RequestedAtUtc { get; private set; }

    public string Purpose { get; private set; }

    public string Status { get; private set; }

    public Guid RequisitionItemId { get; private set; }

    public Guid InventoryItemId { get; private set; }

    public int QuantityRequested { get; private set; }

    public string? ItemNotes { get; private set; }

    public RequisitionReport(
        Guid requisitionId,
        string requisitionNumber,
        Guid requestedByEmployeeId,
        Guid departmentId,
        Guid fiscalYearId,
        DateTime requestedAtUtc,
        string purpose,
        string status,
        Guid requisitionItemId,
        Guid inventoryItemId,
        int quantityRequested,
        string? itemNotes)
    {
        RequisitionId = requisitionId;
        RequisitionNumber = requisitionNumber;
        RequestedByEmployeeId = requestedByEmployeeId;
        DepartmentId = departmentId;
        FiscalYearId = fiscalYearId;
        RequestedAtUtc = requestedAtUtc;
        Purpose = purpose;
        Status = status;
        RequisitionItemId = requisitionItemId;
        InventoryItemId = inventoryItemId;
        QuantityRequested = quantityRequested;
        ItemNotes = itemNotes;
    }
}