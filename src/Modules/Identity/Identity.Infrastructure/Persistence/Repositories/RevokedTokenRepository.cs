
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class RevokedTokenRepository(
    IdentityDbContext dbContext) : IRevokedTokenRepository
{
    public Task<bool> IsRevokedAsync(
        string tokenId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.RevokedTokens
            .AnyAsync(
                token => token.TokenId == tokenId,
                cancellationToken);
    }

    public async Task RevokeAsync(
        string tokenId,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenId);

        var alreadyRevoked = await dbContext.RevokedTokens
            .AnyAsync(
                token => token.TokenId == tokenId,
                cancellationToken);

        if (alreadyRevoked)
        {
            return;
        }

        var revokedToken = new RevokedToken
        {
            TokenId = tokenId,
            ExpiresAtUtc = expiresAtUtc,
            RevokedAtUtc = DateTimeOffset.UtcNow
        };

        await dbContext.RevokedTokens.AddAsync(
            revokedToken,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
