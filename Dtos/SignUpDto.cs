using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Dtos;

public record class SignUpDto(
    [Required][StringLength(50)] string Name,
    [Required][EmailAddress] string Email,
    [Required][StringLength(50)] string Password
);
