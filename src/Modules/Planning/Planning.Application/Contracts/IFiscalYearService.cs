using Planning.Application.DTOs.FiscalYears;

namespace Planning.Application.Contracts;

public interface IFiscalYearService
{
    Task<IReadOnlyList<FiscalYearResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<FiscalYearResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<FiscalYearResponse> CreateAsync(
        CreateFiscalYearRequest request,
        CancellationToken cancellationToken = default);

    Task<FiscalYearResponse?> UpdateAsync(
        Guid id,
        UpdateFiscalYearRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}