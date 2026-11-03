using MediatR;
using TaskFlow.Application.Authentication.DTOs;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Authentication.Commands;

public sealed class RefreshTokenCommandHandler(IRefreshSessionService sessions)
        : IRequestHandler<RefreshTokenCommand, RefreshSessionResult>
{
    private readonly IRefreshSessionService _sessions = sessions;

    public Task<RefreshSessionResult> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        return _sessions.RefreshAsync(
            request.RefreshToken, cancellationToken);
    }
}