namespace TaskFlow.Application.Authentication;

public sealed record IdentityAuthResult(
    bool Succeeded,
    Guid? UserId,
    string? Email,
    string? AccessToken,
    IReadOnlyList<string> Errors);