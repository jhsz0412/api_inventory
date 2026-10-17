
namespace Navigation.Application.DTOs;

public sealed record CreateMenuItemRequest(
    string Name,
    string Label,
    string? Route = null,
    string? Icon = null,
    Guid? ParentMenuItemId = null,
    int SortOrder = 0);
