namespace Inventory.Application.DTOs;

public sealed record CreateUnitOfMeasureRequest(
    string Code,
    string Name);