using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Users.DTOs;

namespace TaskFlow.Application.Users.Commands.UpdateMyProfile;

public sealed class UpdateMyProfileCommandHandler(
    IUserProfileService userProfileService)
        : IRequestHandler<UpdateMyProfileCommand, UserProfileDto?>
{
    private readonly IUserProfileService _userProfileService = userProfileService;

    public Task<UserProfileDto?> Handle(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken)
    {
        return _userProfileService.UpdateAsync(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.AvatarUrl,
            cancellationToken);
    }
}