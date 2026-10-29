using MediatR;

namespace TaskFlow.Application.Authentication.Commands;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthenticationResponse>;