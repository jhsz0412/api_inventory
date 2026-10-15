
namespace Identity.Application.DTOs.Permissions;

public sealed class PermissionResponse
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTimeOffset CreatedUtc { get; init; }
}
