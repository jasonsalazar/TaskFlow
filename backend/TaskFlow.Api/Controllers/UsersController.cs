using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Users.Commands.UpdateMyProfile;
using TaskFlow.Application.Users.Queries.GetMyProfile;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _sender.Send(
            new GetMyProfileQuery(userId),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        UpdateMyProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _sender.Send(
            new UpdateMyProfileCommand(
                userId,
                request.FirstName,
                request.LastName,
                request.AvatarUrl),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}

public sealed record UpdateMyProfileRequest(
    string FirstName,
    string LastName,
    string? AvatarUrl
);