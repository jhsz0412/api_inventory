
namespace Navigation.Application.DTOs;

public sealed record MenuItemResponse(
    Guid Id,
    string Name,
    string Label,
    string? Route,
    string? Icon,
    Guid? ParentMenuItemId,
    Guid? PermissionId,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
