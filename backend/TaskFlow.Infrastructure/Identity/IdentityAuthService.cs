using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Authentication;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Identity;

public sealed class IdentityAuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenService jwtTokenService) : IIdentityAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

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
                false, null, null, null,
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
                false,
                null,
                null,
                null,
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
                false, null, null, null,
                ["The account could not be initialized."]);
        }

        var token = await _jwtTokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email!,
            cancellationToken);

        return new IdentityAuthResult(
            true, user.Id, user.Email, token, []);
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

        var token = await _jwtTokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email!,
            cancellationToken);

        return new IdentityAuthResult(
            true, user.Id, user.Email, token, []);
    }

    private static IdentityAuthResult InvalidCredentials()
    {
        return new IdentityAuthResult(
            false, null, null, null,
            ["Invalid email or password."]);
    }
}