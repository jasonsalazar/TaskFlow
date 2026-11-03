using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Authentication.DTOs;

public sealed record RegisterRequest(
    [Required, StringLength(100, MinimumLength = 1)]
    string FirstName,

    [Required, StringLength(100, MinimumLength = 1)]
    string LastName,

    [Required, EmailAddress, StringLength(256)]
    string Email,

    [Required, StringLength(128, MinimumLength = 8)]
    string Password);