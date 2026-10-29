using MediatR;

namespace TaskFlow.Application.Authentication.Commands;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<AuthenticationResponse>;

public sealed record AuthenticationResponse(
    Guid UserId,
    string Email,
    string AccessToken);