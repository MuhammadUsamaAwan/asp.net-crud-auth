using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;

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
                return Results.Ok(userId);
            }
            var todos = await dbContext.Todos.Where(todo => todo.UserId == Convert.ToInt32(userId)).ToListAsync();
            return Results.Ok(todos);
        }).RequireAuthorization();

        return group;
    }
}
