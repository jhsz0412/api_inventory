namespace Reporting.Domain.Entities;

public sealed class InventoryOverviewSummary
{
    public int TotalInventoryItems { get; private set; }

    public int TotalStockBalances { get; private set; }

    public int LowStockItemCount { get; private set; }

    public int OutOfStockItemCount { get; private set; }

    public InventoryOverviewSummary(
        int totalInventoryItems,
        int totalStockBalances,
        int lowStockItemCount,
        int outOfStockItemCount)
    {
        TotalInventoryItems = totalInventoryItems;
        TotalStockBalances = totalStockBalances;
        LowStockItemCount = lowStockItemCount;
        OutOfStockItemCount = outOfStockItemCount;
    }
}