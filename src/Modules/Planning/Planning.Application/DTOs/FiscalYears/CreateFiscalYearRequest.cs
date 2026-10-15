namespace Planning.Application.DTOs.FiscalYears;

public sealed record CreateFiscalYearRequest(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate);