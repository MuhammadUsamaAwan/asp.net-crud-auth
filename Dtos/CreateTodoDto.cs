using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Dtos;

public record class CreateTodoDto(
    [Required][StringLength(50)] string Text,
    [Required] bool Completed
);
