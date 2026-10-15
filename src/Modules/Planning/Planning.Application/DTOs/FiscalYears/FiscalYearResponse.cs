namespace Planning.Application.DTOs.FiscalYears;

public sealed record FiscalYearResponse(
    Guid Id,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsActive,
    DateTime CreatedAtUtc);