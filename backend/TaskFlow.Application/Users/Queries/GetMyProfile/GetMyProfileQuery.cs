using MediatR;
using TaskFlow.Application.Users.DTOs;

namespace TaskFlow.Application.Users.Queries.GetMyProfile;

public sealed record GetMyProfileQuery(Guid UserId)
    : IRequest<UserProfileDto?>;