using HuynhDuyKhanh_2122110004.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace HuynhDuyKhanh_2122110004.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system";
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }


public static class ProductEndpoints
{
	public static void MapProductEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product").WithTags(nameof(Product));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Products.ToListAsync();
        })
        .WithName("GetAllProducts")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Product>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Products.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Product model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetProductById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Product product, AppDbContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, product.Id)
                  .SetProperty(m => m.Name, product.Name)
                  .SetProperty(m => m.Price, product.Price)
                  .SetProperty(m => m.Description, product.Description)
                  .SetProperty(m => m.ImageUrl, product.ImageUrl)
                  .SetProperty(m => m.Stock, product.Stock)
                  .SetProperty(m => m.BrandId, product.BrandId)
                  .SetProperty(m => m.CreatedAt, product.CreatedAt)
                  .SetProperty(m => m.CreatedBy, product.CreatedBy)
                  .SetProperty(m => m.UpdatedAt, product.UpdatedAt)
                  .SetProperty(m => m.UpdatedBy, product.UpdatedBy)
                  .SetProperty(m => m.CategoryId, product.CategoryId)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateProduct")
        .WithOpenApi();

        group.MapPost("/", async (Product product, AppDbContext db) =>
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Product/{product.Id}",product);
        })
        .WithName("CreateProduct")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteProduct")
        .WithOpenApi();
    }
}}