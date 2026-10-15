
namespace Organization.Application.DTOs.Positions;

public sealed record CreatePositionRequest(
    string Code,
    string Name);
