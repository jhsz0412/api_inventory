
namespace Navigation.Domain.Entities;

public sealed class MenuItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public string Label { get; private set; } = string.Empty;

    public string? Route { get; private set; }

    public string? Icon { get; private set; }

    public Guid? ParentMenuItemId { get; private set; }

    public MenuItem? ParentMenuItem { get; private set; }

    public ICollection<MenuItem> Children { get; private set; } =
        new List<MenuItem>();

    public Guid? PermissionId { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAtUtc { get; private set; } =
        DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    private MenuItem()
    {
        // Used by Entity Framework Core.
    }

    public MenuItem(
        string name,
        string label,
        string? route = null,
        string? icon = null,
        Guid? parentMenuItemId = null,
        Guid? permissionId = null,
        int sortOrder = 0)
    {
        SetName(name);
        SetLabel(label);

        Route = NormalizeOptionalValue(route);
        Icon = NormalizeOptionalValue(icon);
        ParentMenuItemId = parentMenuItemId;
        PermissionId = permissionId;

        SetSortOrder(sortOrder);
    }

    public void Update(
        string name,
        string label,
        string? route,
        string? icon,
        Guid? parentMenuItemId,
        Guid? permissionId,
        int sortOrder)
    {
        SetName(name);
        SetLabel(label);

        Route = NormalizeOptionalValue(route);
        Icon = NormalizeOptionalValue(icon);
        ParentMenuItemId = parentMenuItemId;
        PermissionId = permissionId;

        SetSortOrder(sortOrder);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    private void SetLabel(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Label = label.Trim();
    }

    private void SetSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder),
                "Sort order cannot be negative.");
        }

        SortOrder = sortOrder;
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
