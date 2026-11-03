using TaskFlow.Application.Authentication.DTOs;

namespace TaskFlow.Application.Common.Interfaces;

public interface IRefreshSessionService
{
    Task<RefreshSessionResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken);

    Task RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken);
}