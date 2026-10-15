namespace Inventory.Application.DTOs;

public sealed record UpdateWarehouseRequest(
    string Name,
    string? Description = null);