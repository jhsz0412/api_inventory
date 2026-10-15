namespace Reporting.Domain.Entities;

public sealed class RequisitionStatusSummary
{
    public string Status { get; private set; }

    public int Count { get; private set; }

    public RequisitionStatusSummary(string status, int count)
    {
        Status = status;
        Count = count;
    }
}