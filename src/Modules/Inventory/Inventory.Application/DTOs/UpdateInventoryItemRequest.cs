
namespace Inventory.Application.DTOs;

public sealed record UpdateInventoryItemRequest(
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId,
    string? Description,
    decimal ReorderLevel);
