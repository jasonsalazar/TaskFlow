using MediatR;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Authentication.Commands;

public sealed class LogoutCommandHandler(IRefreshSessionService sessions)
        : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshSessionService _sessions = sessions;

    public async Task<Unit> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        await _sessions.RevokeAsync(
            request.RefreshToken, cancellationToken);

        return Unit.Value;
    }
}