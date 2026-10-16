
using Microsoft.EntityFrameworkCore;
using Planning.Domain.Entities;
using Planning.Infrastructure.Persistence;

namespace InventoryManagement.DatabaseSetup.Seeding;

public static class FiscalYearSeeder
{
    private const string FiscalYearName = "FY 2026";

    private static readonly DateOnly StartDate =
        new(2026, 1, 1);

    private static readonly DateOnly EndDate =
        new(2026, 12, 31);

    public static async Task SeedAsync(
        PlanningDbContext planningDb,
        CancellationToken cancellationToken = default)
    {
        var existing = await planningDb.FiscalYears
            .SingleOrDefaultAsync(
                x => x.Name == FiscalYearName,
                cancellationToken);

        if (existing is not null)
        {
            if (existing.StartDate != StartDate ||
                existing.EndDate != EndDate)
            {
                throw new InvalidOperationException(
                    $"Fiscal year '{FiscalYearName}' already exists " +
                    "with different dates. No changes were made.");
            }

            Console.WriteLine(
                $"Fiscal year already exists: {FiscalYearName}");

            return;
        }

        var overlaps = await planningDb.FiscalYears
            .AnyAsync(
                x => x.StartDate <= EndDate &&
                     x.EndDate >= StartDate,
                cancellationToken);

        if (overlaps)
        {
            throw new InvalidOperationException(
                "Another fiscal year overlaps with FY 2026. " +
                "Resolve the conflicting dates before seeding.");
        }

        var fiscalYear = new FiscalYear(
            FiscalYearName,
            StartDate,
            EndDate);

        planningDb.FiscalYears.Add(fiscalYear);

        await planningDb.SaveChangesAsync(cancellationToken);

        Console.WriteLine(
            $"Fiscal year seeded: {FiscalYearName} " +
            $"({StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd})");
    }
}
