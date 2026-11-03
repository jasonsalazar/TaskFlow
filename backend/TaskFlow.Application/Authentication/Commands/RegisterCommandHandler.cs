using MediatR;
using TaskFlow.Application.Authentication.DTOs;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Authentication.Commands;

public sealed class RegisterCommandHandler(IIdentityAuthService authService)
        : IRequestHandler<RegisterCommand, AuthenticationResponse>
{
    private readonly IIdentityAuthService _authService = authService;

    public async Task<AuthenticationResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            cancellationToken);

        if (!result.Succeeded ||
            result.UserId is null ||
            result.Email is null ||
            result.AccessToken is null ||
            result.RefreshToken is null)
        {
            throw new AuthenticationValidationException(result.Errors);
        }

        return new AuthenticationResponse(
            result.UserId.Value,
            result.Email,
            result.AccessToken,
            result.AccessTokenExpiresAtUtc!.Value,
            result.RefreshToken,
            result.RefreshTokenExpiresAtUtc!.Value);
    }
}