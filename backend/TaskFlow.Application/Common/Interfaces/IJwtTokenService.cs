namespace TaskFlow.Application.Common.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(
        Guid userId,
        string email,
        CancellationToken cancellationToken = default);
}