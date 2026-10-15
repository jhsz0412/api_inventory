
namespace Organization.Domain.Entities;

public sealed class Position
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Position()
    {
        // Required by EF Core.
    }

    public Position(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Position code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Position name is required.", nameof(name));

        Id = Guid.NewGuid();
        Code = code.Trim();
        Name = name.Trim();
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Position code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Position name is required.", nameof(name));

        Code = code.Trim();
        Name = name.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
