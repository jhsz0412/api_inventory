namespace Organization.Application.DTOs.Groups;

public sealed record CreateGroupRequest(
    string Code,
    string Name);
