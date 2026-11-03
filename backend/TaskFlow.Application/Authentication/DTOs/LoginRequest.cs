using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Authentication.DTOs;

public sealed record LoginRequest(
    [Required, EmailAddress, StringLength(256)]
    string Email,

    [Required]
    string Password);