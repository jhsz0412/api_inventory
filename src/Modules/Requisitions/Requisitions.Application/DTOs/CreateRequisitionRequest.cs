namespace Requisitions.Application.DTOs;

public sealed record CreateRequisitionRequest(
    Guid RequestedByEmployeeId,
    Guid DepartmentId,
    Guid FiscalYearId,
    string Purpose);