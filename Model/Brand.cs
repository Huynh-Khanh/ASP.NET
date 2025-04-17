using HuynhDuyKhanh_2122110004.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace HuynhDuyKhanh_2122110004.Model
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system"; // hoặc "admin", hay lấy từ token
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }


public static class BrandEndpoints
{
	public static void MapBrandEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Brand").WithTags(nameof(Brand));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Brands.ToListAsync();
        })
        .WithName("GetAllBrands")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Brand>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Brands.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Brand model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetBrandById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Brand brand, AppDbContext db) =>
        {
            var affected = await db.Brands
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, brand.Id)
                  .SetProperty(m => m.Name, brand.Name)
                  .SetProperty(m => m.CreatedAt, brand.CreatedAt)
                  .SetProperty(m => m.CreatedBy, brand.CreatedBy)
                  .SetProperty(m => m.UpdatedAt, brand.UpdatedAt)
                  .SetProperty(m => m.UpdatedBy, brand.UpdatedBy)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBrand")
        .WithOpenApi();

        group.MapPost("/", async (Brand brand, AppDbContext db) =>
        {
            db.Brands.Add(brand);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Brand/{brand.Id}",brand);
        })
        .WithName("CreateBrand")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Brands
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBrand")
        .WithOpenApi();
    }
}}