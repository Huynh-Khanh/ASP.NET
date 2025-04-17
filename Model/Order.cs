using HuynhDuyKhanh_2122110004.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace HuynhDuyKhanh_2122110004.Model
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system"; // hoặc "admin", hay lấy từ token
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public ICollection<OrderItem>? Items { get; set; }
    }


public static class OrderEndpoints
{
	public static void MapOrderEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Order").WithTags(nameof(Order));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Orders.ToListAsync();
        })
        .WithName("GetAllOrders")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Order>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Orders.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Order model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetOrderById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Order order, AppDbContext db) =>
        {
            var affected = await db.Orders
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, order.Id)
                  .SetProperty(m => m.OrderDate, order.OrderDate)
                  .SetProperty(m => m.TotalAmount, order.TotalAmount)
                  .SetProperty(m => m.CreatedAt, order.CreatedAt)
                  .SetProperty(m => m.CreatedBy, order.CreatedBy)
                  .SetProperty(m => m.UpdatedAt, order.UpdatedAt)
                  .SetProperty(m => m.UpdatedBy, order.UpdatedBy)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOrder")
        .WithOpenApi();

        group.MapPost("/", async (Order order, AppDbContext db) =>
        {
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Order/{order.Id}",order);
        })
        .WithName("CreateOrder")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Orders
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteOrder")
        .WithOpenApi();
    }
}}