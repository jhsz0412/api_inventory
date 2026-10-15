namespace Reporting.Application.DTOs;

public sealed class RequisitionReportRequest
{
    public Guid? RequestedByEmployeeId { get; init; }

    public Guid? DepartmentId { get; init; }

    public Guid? FiscalYearId { get; init; }

    public string? Status { get; init; }

    public DateTime? FromUtc { get; init; }

    public DateTime? ToUtc { get; init; }
}