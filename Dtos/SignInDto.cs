using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Dtos;

public record class SignInDto(
    [Required][EmailAddress] string Email,
    [Required][StringLength(50)] string Password
);
