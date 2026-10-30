using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Authentication;

namespace TaskFlow.Infrastructure.Persistence;

public sealed class RefreshTokenRepository(
    ApplicationDbContext dbContext)
        : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task AddAsync(
        RefreshToken token,
        CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens.AddAsync(
            token, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task<bool> TryRotateAsync(
        string currentTokenHash,
        RefreshToken replacementToken,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var now = DateTime.UtcNow;

        // Conditional update means only one concurrent request
        // can revoke and replace this active token.
        var affected = await _dbContext.RefreshTokens
            .Where(token =>
                token.TokenHash == currentTokenHash &&
                token.RevokedAtUtc == null &&
                token.ExpiresAtUtc > now &&
                token.UserId == replacementToken.UserId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        token => token.RevokedAtUtc,
                        now)
                    .SetProperty(
                        token => token.ReplacedByTokenHash,
                        replacementToken.TokenHash),
                cancellationToken);

        if (affected != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        await _dbContext.RefreshTokens.AddAsync(
            replacementToken, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> TryRevokeAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var affected = await _dbContext.RefreshTokens
            .Where(token =>
                token.TokenHash == tokenHash &&
                token.RevokedAtUtc == null &&
                token.ExpiresAtUtc > now)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    token => token.RevokedAtUtc,
                    now),
                cancellationToken);

        return affected == 1;
    }
}