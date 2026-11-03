using MediatR;
using TaskFlow.Application.Authentication.DTOs;

namespace TaskFlow.Application.Authentication.Commands;

public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<RefreshSessionResult>;