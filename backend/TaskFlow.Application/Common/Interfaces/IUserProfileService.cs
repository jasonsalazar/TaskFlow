using TaskFlow.Application.Users.DTOs;

namespace TaskFlow.Application.Common.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserProfileDto?> UpdateAsync(
        Guid userId,
        string firstName,
        string lastName,
        string? avatarUrl,
        CancellationToken cancellationToken = default);
}