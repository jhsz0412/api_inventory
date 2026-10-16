
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class RevokedTokenConfiguration
    : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.ToTable("RevokedTokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.TokenId)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(token => token.TokenId)
            .IsUnique();

        builder.Property(token => token.ExpiresAtUtc)
            .IsRequired();

        builder.Property(token => token.RevokedAtUtc)
            .IsRequired();

        builder.HasIndex(token => token.ExpiresAtUtc);
    }
}
