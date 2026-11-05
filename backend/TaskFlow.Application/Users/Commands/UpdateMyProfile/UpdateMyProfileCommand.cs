using MediatR;
using TaskFlow.Application.Users.DTOs;

namespace TaskFlow.Application.Users.Commands.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? AvatarUrl
) : IRequest<UserProfileDto?>;