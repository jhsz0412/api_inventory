
namespace Planning.Domain.Entities;

public sealed class FiscalYear
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private FiscalYear()
    {
    }

    public FiscalYear(
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        Validate(name, startDate, endDate);

        Id = Guid.NewGuid();
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        Validate(name, startDate, endDate);

        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static void Validate(
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Fiscal year name is required.", nameof(name));

        if (name.Trim().Length > 100)
            throw new ArgumentException(
                "Fiscal year name cannot exceed 100 characters.",
                nameof(name));

        if (endDate < startDate)
            throw new ArgumentException(
                "Fiscal year end date cannot be earlier than its start date.");
    }
}
