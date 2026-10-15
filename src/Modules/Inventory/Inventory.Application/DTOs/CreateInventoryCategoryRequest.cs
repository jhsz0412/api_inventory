namespace Inventory.Application.DTOs;

public sealed record CreateInventoryCategoryRequest(
    string Code,
    string Name,
    string? Description);