
namespace Navigation.Application.DTOs;

public sealed record CreateMenuItemRequest(
    string Name,
    string Label,
    Guid GroupId,
    string? Route = null,
    string? Icon = null,
    Guid? ParentMenuItemId = null,
    int SortOrder = 0);
