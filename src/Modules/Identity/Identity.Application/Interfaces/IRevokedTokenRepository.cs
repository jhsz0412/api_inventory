
namespace Identity.Application.Interfaces;

public interface IRevokedTokenRepository
{
    Task<bool> IsRevokedAsync(
        string tokenId,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(
        string tokenId,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken = default);
}
