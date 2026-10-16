
namespace Identity.Domain.Entities;

public sealed class RevokedToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string TokenId { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public DateTimeOffset RevokedAtUtc { get; set; } =
        DateTimeOffset.UtcNow;
}
