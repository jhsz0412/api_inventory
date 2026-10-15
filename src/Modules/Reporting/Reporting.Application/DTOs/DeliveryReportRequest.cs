namespace Reporting.Application.DTOs;

public sealed class DeliveryReportRequest
{
    public Guid? RequisitionId { get; init; }

    public string? Status { get; init; }

    public Guid? PreparedByEmployeeId { get; init; }

    public Guid? DispatchedByEmployeeId { get; init; }

    public DateTime? FromUtc { get; init; }

    public DateTime? ToUtc { get; init; }
}
