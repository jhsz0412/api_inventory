
namespace Identity.Application.DTOs.Roles;

public sealed class RoleResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTimeOffset CreatedUtc { get; init; }
}
