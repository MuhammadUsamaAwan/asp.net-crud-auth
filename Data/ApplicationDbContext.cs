using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Entities;

namespace TodoApp.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Todo> Todos => Set<Todo>();
}
