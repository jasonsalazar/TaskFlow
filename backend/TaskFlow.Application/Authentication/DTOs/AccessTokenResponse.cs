namespace TaskFlow.Application.Authentication.DTOs;

public sealed record AccessTokenResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc);