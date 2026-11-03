using MediatR;

namespace TaskFlow.Application.Authentication.Commands;

public sealed record LogoutCommand(string RefreshToken)
    : IRequest<Unit>;