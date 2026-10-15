namespace Inventory.Application.DTOs;

public sealed record CreateWarehouseRequest(
    string Code,
    string Name,
    string? Description = null);