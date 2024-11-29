using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Dtos;
using TodoApp.Api.Entities;

namespace TodoApp.Api.Endpoints;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodoEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("todos");

        group.MapGet("/", async (ApplicationDbContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Results.Unauthorized();
            }
            var todos = await dbContext.Todos.Where(todo => todo.UserId == Convert.ToInt32(userId)).ToListAsync();
            return Results.Ok(todos);
        }).RequireAuthorization();

        group.MapPost("/", async (ApplicationDbContext dbContext, ClaimsPrincipal user, CreateTodoDto input) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Results.Unauthorized();
            }
            var _user = await dbContext.Users.FindAsync(Convert.ToInt32(userId));
            if (_user == null)
            {
                return Results.Unauthorized();
            }
            Todo newTodo = new()
            {
                Text = input.Text,
                Completed = input.Completed,
                UserId = _user.Id,
                User = _user
            };
            await dbContext.AddAsync(newTodo);
            await dbContext.SaveChangesAsync();
            return Results.Ok(newTodo);
        }).RequireAuthorization();

        return group;
    }
}
