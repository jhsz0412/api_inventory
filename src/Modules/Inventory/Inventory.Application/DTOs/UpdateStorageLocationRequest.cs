namespace Inventory.Application.DTOs;

public sealed record UpdateStorageLocationRequest(
    string Name,
    string? Description = null);
