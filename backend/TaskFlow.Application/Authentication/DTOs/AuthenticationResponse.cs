namespace TaskFlow.Application.Authentication.DTOs;

public sealed record AuthenticationResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);