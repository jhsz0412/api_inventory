
namespace Navigation.Domain.Entities;

public sealed class GroupMenuItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid GroupId { get; private set; }

    public Guid MenuItemId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; } =
        DateTimeOffset.UtcNow;

    private GroupMenuItem()
    {
        // Required by Entity Framework Core.
    }

    public GroupMenuItem(Guid groupId, Guid menuItemId)
    {
        if (groupId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid group is required.", nameof(groupId));
        }

        if (menuItemId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid menu item is required.", nameof(menuItemId));
        }

        Id = Guid.NewGuid();
        GroupId = groupId;
        MenuItemId = menuItemId;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }
}
