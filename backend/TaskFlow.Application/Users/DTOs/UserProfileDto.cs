namespace TaskFlow.Application.Users.DTOs;

public sealed record UserProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    DateTime CreatedAtUtc
);