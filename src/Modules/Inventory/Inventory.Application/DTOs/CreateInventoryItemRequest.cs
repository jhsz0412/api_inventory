namespace Inventory.Application.DTOs;

public sealed record CreateInventoryItemRequest(
    string ItemCode,
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId,
    string? Description,
    decimal ReorderLevel);