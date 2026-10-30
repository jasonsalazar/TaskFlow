using TaskFlow.Domain.Authentication;

namespace TaskFlow.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(
        RefreshToken token,
        CancellationToken cancellationToken);

    Task<RefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task<bool> TryRotateAsync(
        string currentTokenHash,
        RefreshToken replacementToken,
        CancellationToken cancellationToken);

    Task<bool> TryRevokeAsync(
        string tokenHash,
        CancellationToken cancellationToken);
}