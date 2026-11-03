using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TaskFlow.Application.Authentication.DTOs;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Authentication;

namespace TaskFlow.Infrastructure.Identity;

public sealed class RefreshSessionService(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenRepository repository,
    IRefreshTokenService tokenService,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> options) : IRefreshSessionService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IRefreshTokenRepository _repository = repository;
    private readonly IRefreshTokenService _tokenService = tokenService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly JwtOptions _options = options.Value;

    public async Task<RefreshSessionResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Failed();

        var currentHash = _tokenService.HashToken(refreshToken);

        var current = await _repository.GetByHashAsync(
            currentHash, cancellationToken);

        if (current is null || !current.IsActive)
            return Failed();

        var user = await _userManager.FindByIdAsync(
            current.UserId.ToString());

        if (user is null || !user.IsActive)
            return Failed();

        var rawReplacement = _tokenService.GenerateToken();
        var replacementHash = _tokenService.HashToken(rawReplacement);
        var now = DateTime.UtcNow;
        var accessTokenExpiresAt = now.AddMinutes(
            _options.AccessTokenMinutes);
        var refreshTokenExpiresAt = now.AddDays(
            _options.RefreshTokenDays);

        var replacement = new RefreshToken(
            user.Id,
            replacementHash,
            refreshTokenExpiresAt);

        // Generate the JWT before rotating, so a token-generation
        // failure doesn't invalidate the existing refresh token.
        var accessToken = await _jwtTokenService
            .GenerateAccessTokenAsync(user.Id, user.Email!, cancellationToken);

        var rotated = await _repository.TryRotateAsync(
            currentHash, replacement, cancellationToken);

        if (!rotated)
            return Failed();

        return new RefreshSessionResult(
            true,
            user.Id,
            user.Email,
            accessToken,
            rawReplacement,
            accessTokenExpiresAt,
            refreshTokenExpiresAt);
    }

    public async Task RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        var hash = _tokenService.HashToken(refreshToken);

        await _repository.TryRevokeAsync(hash, cancellationToken);
    }

    private static RefreshSessionResult Failed() =>
        new(
            false,
            null,
            null,
            null,
            null,
            null,
            null);
}