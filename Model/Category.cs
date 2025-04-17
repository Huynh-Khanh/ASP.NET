using HuynhDuyKhanh_2122110004.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace HuynhDuyKhanh_2122110004.Model
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system"; 
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public ICollection<Product>? Products { get; set; }
    }


public static class CategoryEndpoints
{
	public static void MapCategoryEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Category").WithTags(nameof(Category));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Categories.ToListAsync();
        })
        .WithName("GetAllCategories")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Category>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Categories.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Category model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetCategoryById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Category category, AppDbContext db) =>
        {
            var affected = await db.Categories
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, category.Id)
                  .SetProperty(m => m.Name, category.Name)
                  .SetProperty(m => m.CreatedAt, category.CreatedAt)
                  .SetProperty(m => m.CreatedBy, category.CreatedBy)
                  .SetProperty(m => m.UpdatedAt, category.UpdatedAt)
                  .SetProperty(m => m.UpdatedBy, category.UpdatedBy)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateCategory")
        .WithOpenApi();

        group.MapPost("/", async (Category category, AppDbContext db) =>
        {
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Category/{category.Id}",category);
        })
        .WithName("CreateCategory")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Categories
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteCategory")
        .WithOpenApi();
    }
}}