using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Users.DTOs;

namespace TaskFlow.Infrastructure.Identity;

public sealed class UserProfileService(
    UserManager<ApplicationUser> userManager) : IUserProfileService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<UserProfileDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken);

        if (user is null)
            return null;

        return MapToDto(user);
    }

    public async Task<UserProfileDto?> UpdateAsync(
        Guid userId,
        string firstName,
        string lastName,
        string? avatarUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
            return null;

        user.FirstName = firstName.Trim();
        user.LastName = lastName.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl)
            ? null
            : avatarUrl.Trim();

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(x => x.Description)));
        }

        return MapToDto(user);
    }

    private static UserProfileDto MapToDto(
        ApplicationUser user)
    {
        return new UserProfileDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.AvatarUrl,
            user.CreatedAtUtc);
    }
}