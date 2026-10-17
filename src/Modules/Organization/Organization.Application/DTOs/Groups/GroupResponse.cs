
namespace Organization.Application.DTOs.Groups;

public sealed record GroupResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);
