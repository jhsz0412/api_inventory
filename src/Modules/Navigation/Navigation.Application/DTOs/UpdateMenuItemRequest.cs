
namespace Navigation.Application.DTOs;

public sealed record UpdateMenuItemRequest(
    string Name,
    string Label,
    string? Route,
    string? Icon,
    Guid? ParentMenuItemId,
    Guid? PermissionId,
    int SortOrder);
