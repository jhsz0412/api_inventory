namespace Requisitions.Domain.Entities;

public sealed class RequisitionNumberSequence
{
    public int Year { get; private set; }

    public int Month { get; private set; }

    public int LastNumber { get; private set; }

    private RequisitionNumberSequence()
    {
    }

    public RequisitionNumberSequence(int year, int month)
    {
        if (year < 1 || year > 9999)
            throw new ArgumentOutOfRangeException(nameof(year));

        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        Year = year;
        Month = month;
        LastNumber = 0;
    }

    public int GetNextNumber()
    {
        if (LastNumber == int.MaxValue)
            throw new InvalidOperationException(
                "The monthly requisition number sequence is exhausted.");

        LastNumber++;
        return LastNumber;
    }
}