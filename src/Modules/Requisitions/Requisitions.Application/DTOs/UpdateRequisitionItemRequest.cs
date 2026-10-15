namespace Requisitions.Application.DTOs;

public sealed record UpdateRequisitionItemRequest(
    int QuantityRequested,
    string? Notes);