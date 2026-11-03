using MediatR;
using TaskFlow.Application.Authentication.DTOs;

namespace TaskFlow.Application.Authentication.Commands;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<AuthenticationResponse>;