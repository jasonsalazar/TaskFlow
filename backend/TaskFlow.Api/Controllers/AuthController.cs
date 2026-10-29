using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Authentication.Commands;

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

            return Ok(response);
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

            return Ok(response);
        }
        catch (AuthenticationValidationException ex)
        {
            return Unauthorized(new { errors = ex.Errors });
        }
    }
}

public sealed record RegisterRequest(
    [Required, StringLength(100, MinimumLength = 1)]
    string FirstName,

    [Required, StringLength(100, MinimumLength = 1)]
    string LastName,

    [Required, EmailAddress, StringLength(256)]
    string Email,

    [Required, StringLength(128, MinimumLength = 8)]
    string Password);

public sealed record LoginRequest(
    [Required, EmailAddress, StringLength(256)]
    string Email,

    [Required]
    string Password);