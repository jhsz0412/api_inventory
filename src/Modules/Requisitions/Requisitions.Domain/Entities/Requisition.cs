namespace Requisitions.Domain.Entities;

using Requisitions.Domain.Enums;

public sealed class Requisition
{
    public Guid Id { get; private set; }

    public string RequisitionNumber { get; private set; } = string.Empty;

    public Guid RequestedByEmployeeId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid FiscalYearId { get; private set; }

    public DateTime RequestedAtUtc { get; private set; }

    public string Purpose { get; private set; } = string.Empty;

    public RequisitionStatus Status { get; private set; } = RequisitionStatus.Draft;

    public byte[] RowVersion { get; private set; } = [];

    public IReadOnlyCollection<RequisitionItem> Items => _items.AsReadOnly();

    private readonly List<RequisitionItem> _items = [];

    private Requisition()
    {
    }

    public Requisition(
        Guid requestedByEmployeeId,
        Guid departmentId,
        Guid fiscalYearId,
        string purpose)
    {
        if (requestedByEmployeeId == Guid.Empty)
            throw new ArgumentException(
                "Requesting employee is required.",
                nameof(requestedByEmployeeId));

        if (departmentId == Guid.Empty)
            throw new ArgumentException(
                "Department is required.",
                nameof(departmentId));

        if (fiscalYearId == Guid.Empty)
            throw new ArgumentException(
                "Fiscal year is required.",
                nameof(fiscalYearId));

        if (string.IsNullOrWhiteSpace(purpose))
            throw new ArgumentException(
                "Requisition purpose is required.",
                nameof(purpose));

        if (purpose.Trim().Length > 500)
            throw new ArgumentException(
                "Purpose cannot exceed 500 characters.",
                nameof(purpose));

        Id = Guid.NewGuid();
        RequestedByEmployeeId = requestedByEmployeeId;
        DepartmentId = departmentId;
        FiscalYearId = fiscalYearId;
        Purpose = purpose.Trim();
        RequestedAtUtc = DateTime.UtcNow;
    }


    public void AssignRequisitionNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException(
                    "Requisition number is required.",
                    nameof(number));

            if (number.Trim().Length > 30)
                throw new ArgumentException(
                    "Requisition number cannot exceed 30 characters.",
                    nameof(number));

            if (!string.IsNullOrEmpty(RequisitionNumber))
                throw new InvalidOperationException(
                    "A requisition number has already been assigned.");

            RequisitionNumber = number.Trim();
        }

    public void AddItem(RequisitionItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.RequisitionId != Id)
            throw new ArgumentException(
                "The item belongs to a different requisition.",
                nameof(item));

        if (_items.Any(existing =>
                existing.InventoryItemId == item.InventoryItemId))
            throw new InvalidOperationException(
                "This inventory item is already included in the requisition.");

        _items.Add(item);
    }

    public void RemoveItem(Guid requisitionItemId)
    {
        var item = _items.FirstOrDefault(
            existing => existing.Id == requisitionItemId);

        if (item is null)
            throw new InvalidOperationException(
                "The requisition item was not found.");

        _items.Remove(item);
    }

    public void Submit()
    {
        if (Status != RequisitionStatus.Draft)
            throw new InvalidOperationException(
                "Only draft requisitions can be submitted.");

        if (_items.Count == 0)
            throw new InvalidOperationException(
                "A requisition must contain at least one item.");

        Status = RequisitionStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != RequisitionStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted requisitions can be approved.");

        Status = RequisitionStatus.Approved;
    }

    public void Reject()
    {
        if (Status != RequisitionStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted requisitions can be rejected.");

        Status = RequisitionStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status != RequisitionStatus.Draft &&
            Status != RequisitionStatus.Submitted)
            throw new InvalidOperationException(
                "Only draft or submitted requisitions can be cancelled.");

        Status = RequisitionStatus.Cancelled;
    }


    public void BeginFulfillment()
    {
        if (Status != RequisitionStatus.Approved)
            throw new InvalidOperationException(
                "Only approved requisitions can begin fulfillment.");

        Status = RequisitionStatus.FulfillmentInProgress;
    }

    public void MarkFulfilled()
    {
        if (Status != RequisitionStatus.FulfillmentInProgress)
            throw new InvalidOperationException(
                "Only requisitions in progress can be marked fulfilled.");

        Status = RequisitionStatus.Fulfilled;
    }

    public void RestoreApprovedStatus()
    {
        if (Status != RequisitionStatus.FulfillmentInProgress)
            throw new InvalidOperationException(
                "Only requisitions in progress can be restored to approved.");

        Status = RequisitionStatus.Approved;
    }

}