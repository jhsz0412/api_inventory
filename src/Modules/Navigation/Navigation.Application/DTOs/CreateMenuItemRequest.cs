
namespace Navigation.Application.DTOs;

public sealed record CreateMenuItemRequest(
    string Name,
    string Label,
    string? Route = null,
    string? Icon = null,
    Guid? ParentMenuItemId = null,
    Guid? PermissionId = null,
    int SortOrder = 0);
