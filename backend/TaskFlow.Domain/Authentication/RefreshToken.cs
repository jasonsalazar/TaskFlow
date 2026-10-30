namespace TaskFlow.Domain.Authentication;

public sealed class RefreshToken
{
    private RefreshToken() { } // EF Core

    public RefreshToken(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsActive =>
        RevokedAtUtc is null &&
        ExpiresAtUtc > DateTime.UtcNow;

    public void Revoke(
        string? replacementTokenHash = null)
    {
        if (RevokedAtUtc is not null)
            throw new InvalidOperationException(
                "Refresh token has already been revoked.");

        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenHash = replacementTokenHash;
    }
}