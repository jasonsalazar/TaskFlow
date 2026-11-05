using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Users.DTOs;

namespace TaskFlow.Application.Users.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler(
    IUserProfileService userProfileService)
        : IRequestHandler<GetMyProfileQuery, UserProfileDto?>
{
    private readonly IUserProfileService _userProfileService = userProfileService;

    public Task<UserProfileDto?> Handle(
        GetMyProfileQuery request,
        CancellationToken cancellationToken)
    {
        return _userProfileService.GetByIdAsync(
            request.UserId,
            cancellationToken);
    }
}