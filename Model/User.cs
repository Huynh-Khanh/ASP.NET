using HuynhDuyKhanh_2122110004.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace HuynhDuyKhanh_2122110004.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system"; // hoặc "admin", hay lấy từ token
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }


public static class UserEndpoints
{
	public static void MapUserEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User").WithTags(nameof(User));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Users.ToListAsync();
        })
        .WithName("GetAllUsers")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is User model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetUserById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, User user, AppDbContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, user.Id)
                  .SetProperty(m => m.Username, user.Username)
                  .SetProperty(m => m.Password, user.Password)
                  .SetProperty(m => m.Email, user.Email)
                  .SetProperty(m => m.FullName, user.FullName)
                  .SetProperty(m => m.CreatedAt, user.CreatedAt)
                  .SetProperty(m => m.CreatedBy, user.CreatedBy)
                  .SetProperty(m => m.UpdatedAt, user.UpdatedAt)
                  .SetProperty(m => m.UpdatedBy, user.UpdatedBy)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        group.MapPost("/", async (User user, AppDbContext db) =>
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/User/{user.Id}",user);
        })
        .WithName("CreateUser")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteUser")
        .WithOpenApi();
    }
}}