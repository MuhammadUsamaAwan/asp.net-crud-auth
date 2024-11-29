namespace TodoApp.Api.Entities;

public class Todo
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public bool Completed { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
}
