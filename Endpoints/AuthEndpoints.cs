using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Api.Data;
using TodoApp.Api.Dtos;
using TodoApp.Api.Entities;

namespace TodoApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static string GenerateToken(User user, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Email, user.Email)],
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }

    public static WebApplication MapAuthEndPoints(this WebApplication app)
    {
        app.MapPost("/signup", async (SignUpDto input, ApplicationDbContext dbContext, IConfiguration configuration) =>
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == input.Email);
            if (existingUser != null)
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
            var tokenString = GenerateToken(newUser, configuration);
            return Results.Ok(new { Token = tokenString });
        }).WithParameterValidation();

        app.MapPost("/signin", async (SignInDto input, ApplicationDbContext dbContext, IConfiguration configuration) =>
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == input.Email);
            if (user == null)
            {
                return Results.Unauthorized();
            }
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(input.Password, user.Hash);
            if (isPasswordCorrect == false)
            {
                return Results.Unauthorized();
            }
            var tokenString = GenerateToken(user, configuration);
            return Results.Ok(new { Token = tokenString });
        }).WithParameterValidation();

        return app;
    }
}
