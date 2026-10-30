using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TaskFlow.Application.Authentication;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Authentication;

namespace TaskFlow.Infrastructure.Identity;

public sealed class IdentityAuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions) : IIdentityAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<IdentityAuthResult> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();

        var existingUser = await _userManager
            .FindByEmailAsync(normalizedEmail);

        if (existingUser is not null)
        {
            return new IdentityAuthResult(
                false, null, null, null, null, null, null,
                ["An account with this email already exists."]);
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(
            user, password);

        if (!createResult.Succeeded)
        {
            return new IdentityAuthResult(
                false, null, null, null, null, null, null,
                [.. createResult.Errors.Select(error => error.Description)]);
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user, "User");

        if (!roleResult.Succeeded)
        {
            // Do not leave an account behind if initial role
            // assignment fails.
            await _userManager.DeleteAsync(user);

            return new IdentityAuthResult(
                false, null, null, null, null, null, null,
                ["The account could not be initialized."]);
        }

        return await CreateSessionAsync(user, cancellationToken);
    }

    public async Task<IdentityAuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());

        if (user is null || !user.IsActive)
        {
            return InvalidCredentials();
        }

        var passwordResult =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

        if (!passwordResult.Succeeded)
        {
            return InvalidCredentials();
        }

        return await CreateSessionAsync(user, cancellationToken);
    }

    private async Task<IdentityAuthResult> CreateSessionAsync(
    ApplicationUser user,
    CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var accessToken = await _jwtTokenService
            .GenerateAccessTokenAsync(user.Id, user.Email!);

        var rawRefreshToken = _refreshTokenService.GenerateToken();
        var refreshTokenHash = _refreshTokenService
            .HashToken(rawRefreshToken);

        var refreshTokenExpiresAtUtc =
            now.AddDays(_jwtOptions.RefreshTokenDays);

        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenHash,
            refreshTokenExpiresAtUtc);

        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        return new IdentityAuthResult(
            true,
            user.Id,
            user.Email,
            accessToken,
            rawRefreshToken,
            now.AddMinutes(_jwtOptions.AccessTokenMinutes),
            refreshTokenExpiresAtUtc,
            []);
    }

    private static IdentityAuthResult InvalidCredentials()
    {
        return new IdentityAuthResult(
            false, null, null, null, null, null, null,
            ["Invalid email or password."]);
    }
}