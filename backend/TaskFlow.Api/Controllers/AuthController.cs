using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Authentication.Commands;
using TaskFlow.Application.Authentication.DTOs;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _sender.Send(
                new RegisterCommand(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Password),
                cancellationToken);

            SetRefreshCookie(response.RefreshToken, response.RefreshTokenExpiresAtUtc);

            return Ok(new AccessTokenResponse(
                response.UserId,
                response.Email,
                response.AccessToken,
                response.AccessTokenExpiresAtUtc));
        }
        catch (AuthenticationValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _sender.Send(
                new LoginCommand(request.Email, request.Password),
                cancellationToken);

            SetRefreshCookie(response.RefreshToken, response.RefreshTokenExpiresAtUtc);

            return Ok(new AccessTokenResponse(
                response.UserId,
                response.Email,
                response.AccessToken,
                response.AccessTokenExpiresAtUtc));
        }
        catch (AuthenticationValidationException ex)
        {
            return Unauthorized(new { errors = ex.Errors });
        }
    }

    private const string RefreshCookieName = "taskflow.refresh";

    private void SetRefreshCookie(
        string token,
        DateTime expiresAtUtc)
    {
        Response.Cookies.Append(
            RefreshCookieName,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = new DateTimeOffset(
                    DateTime.SpecifyKind(
                        expiresAtUtc, DateTimeKind.Utc)),
                Path = "/api/auth",
                IsEssential = true,
            });
    }

    private void ClearRefreshCookie()
    {
        Response.Cookies.Delete(
            RefreshCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth"
            });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(
                RefreshCookieName, out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new
            {
                message = "A valid refresh session is required."
            });
        }

        var result = await _sender.Send(
            new RefreshTokenCommand(refreshToken),
            cancellationToken);

        if (!result.Succeeded ||
            result.AccessToken is null ||
            result.RefreshToken is null ||
            result.UserId is null ||
            result.Email is null ||
            result.AccessTokenExpiresAtUtc is null ||
            result.RefreshTokenExpiresAtUtc is null)
        {
            ClearRefreshCookie();

            return Unauthorized(new
            {
                message = "The refresh session is invalid or expired."
            });
        }

        SetRefreshCookie(
            result.RefreshToken,
            result.RefreshTokenExpiresAtUtc.Value);

        return Ok(new AccessTokenResponse(
            result.UserId.Value,
            result.Email,
            result.AccessToken,
            result.AccessTokenExpiresAtUtc.Value));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(
                RefreshCookieName, out var refreshToken) &&
            !string.IsNullOrWhiteSpace(refreshToken))
        {
            await _sender.Send(
                new LogoutCommand(refreshToken),
                cancellationToken);
        }

        ClearRefreshCookie();

        return NoContent();
    }
}