using Planning.Application.Contracts;
using Planning.Application.DTOs.FiscalYears;
using Planning.Domain.Entities;

namespace Planning.Application.Services.FiscalYears;

public sealed class FiscalYearService(
    IFiscalYearRepository repository) : IFiscalYearService
{
    public async Task<IReadOnlyList<FiscalYearResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var fiscalYears = await repository.GetAllAsync(cancellationToken);
        return fiscalYears.Select(MapToResponse).ToList();
    }

    public async Task<FiscalYearResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var fiscalYear = await repository.GetByIdAsync(id, cancellationToken);
        return fiscalYear is null ? null : MapToResponse(fiscalYear);
    }

    public async Task<FiscalYearResponse> CreateAsync(
        CreateFiscalYearRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateDateRange(request.StartDate, request.EndDate);

        if (await repository.ExistsByNameAsync(
                request.Name, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A fiscal year named '{request.Name}' already exists.");
        }

        if (await repository.HasOverlapAsync(
                request.StartDate,
                request.EndDate,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                "The date range overlaps with an existing fiscal year.");
        }

        var fiscalYear = new FiscalYear(
            request.Name, request.StartDate, request.EndDate);

        await repository.AddAsync(fiscalYear, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(fiscalYear);
    }

    public async Task<FiscalYearResponse?> UpdateAsync(
        Guid id,
        UpdateFiscalYearRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateDateRange(request.StartDate, request.EndDate);

        var fiscalYear = await repository.GetByIdAsync(id, cancellationToken);
        if (fiscalYear is null)
            return null;

        if (await repository.ExistsByNameAsync(
                request.Name, excludeId: id,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A fiscal year named '{request.Name}' already exists.");
        }

        if (await repository.HasOverlapAsync(
                request.StartDate, request.EndDate,
                excludeId: id, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                "The date range overlaps with another fiscal year.");
        }

        fiscalYear.UpdateDetails(
            request.Name, request.StartDate, request.EndDate);

        await repository.SaveChangesAsync(cancellationToken);
        return MapToResponse(fiscalYear);
    }

    public async Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var fiscalYear = await repository.GetByIdAsync(id, cancellationToken);
        if (fiscalYear is null)
            return false;

        fiscalYear.Activate();
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var fiscalYear = await repository.GetByIdAsync(id, cancellationToken);
        if (fiscalYear is null)
            return false;

        fiscalYear.Deactivate();
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ValidateDateRange(
        DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException(
                "Fiscal year end date cannot be earlier than its start date.");
    }

    private static FiscalYearResponse MapToResponse(FiscalYear fiscalYear)
    {
        return new FiscalYearResponse(
            fiscalYear.Id,
            fiscalYear.Name,
            fiscalYear.StartDate,
            fiscalYear.EndDate,
            fiscalYear.IsActive,
            fiscalYear.CreatedAtUtc);
    }
}