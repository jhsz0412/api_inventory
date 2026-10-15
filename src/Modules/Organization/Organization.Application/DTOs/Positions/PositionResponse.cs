
namespace Organization.Application.DTOs.Positions;

public sealed record PositionResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);
