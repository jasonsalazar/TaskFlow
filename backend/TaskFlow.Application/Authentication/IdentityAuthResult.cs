namespace TaskFlow.Application.Authentication;

public sealed record IdentityAuthResult(
    bool Succeeded,
    Guid? UserId,
    string? Email,
    string? AccessToken,
    string? RefreshToken,
    DateTime? AccessTokenExpiresAtUtc,
    DateTime? RefreshTokenExpiresAtUtc,
    IReadOnlyList<string> Errors);