namespace Inventory.Application.DTOs;

public sealed record CreateStorageLocationRequest(
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description = null);