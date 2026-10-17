
namespace Organization.Domain.Entities;

public sealed class GroupMember
{
    public Guid Id { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private GroupMember()
    {
        // Required by EF Core.
    }

    public GroupMember(Guid groupId, Guid employeeId)
    {
        if (groupId == Guid.Empty)
            throw new ArgumentException(
                "A valid group is required.", nameof(groupId));

        if (employeeId == Guid.Empty)
            throw new ArgumentException(
                "A valid employee is required.", nameof(employeeId));

        Id = Guid.NewGuid();
        GroupId = groupId;
        EmployeeId = employeeId;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
