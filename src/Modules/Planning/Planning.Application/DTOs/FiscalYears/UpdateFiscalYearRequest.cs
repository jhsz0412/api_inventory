namespace Planning.Application.DTOs.FiscalYears;

public sealed record UpdateFiscalYearRequest(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate);