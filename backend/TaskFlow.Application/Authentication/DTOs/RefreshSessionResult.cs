namespace TaskFlow.Application.Authentication.DTOs;

public sealed record RefreshSessionResult(
    bool Succeeded,
    Guid? UserId,
    string? Email,
    string? AccessToken,
    string? RefreshToken,
    DateTime? AccessTokenExpiresAtUtc,
    DateTime? RefreshTokenExpiresAtUtc);