using TodoApp.Api.Data;
using TodoApp.Api.Dtos;
using TodoApp.Api.Entities;

namespace TodoApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndPoints(this WebApplication app)
    {
        app.MapPost("/signup", async (SignUpDto input, ApplicationDbContext dbContext) =>
        {
            var ExistingUser = await dbContext.Users.FindAsync(input.Email);
            if (ExistingUser != null)
            {
                return Results.BadRequest();
            }
            User newUser = new()
            {
                Name = input.Name,
                Email = input.Email,
                Hash = BCrypt.Net.BCrypt.HashPassword(input.Password)
            };
            await dbContext.AddAsync(newUser);
            await dbContext.SaveChangesAsync();
            // TODO: sign token and send in response
            return Results.NoContent();
        });

        return app;
    }
}
