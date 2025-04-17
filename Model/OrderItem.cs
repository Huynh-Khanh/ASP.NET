using HuynhDuyKhanh_2122110004.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace HuynhDuyKhanh_2122110004.Model
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system"; // hoặc "admin", hay lấy từ token
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }


public static class OrderItemEndpoints
{
	public static void MapOrderItemEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/OrderItem").WithTags(nameof(OrderItem));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.OrderItems.ToListAsync();
        })
        .WithName("GetAllOrderItems")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<OrderItem>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.OrderItems.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is OrderItem model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetOrderItemById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, OrderItem orderItem, AppDbContext db) =>
        {
            var affected = await db.OrderItems
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, orderItem.Id)
                  .SetProperty(m => m.OrderId, orderItem.OrderId)
                  .SetProperty(m => m.ProductId, orderItem.ProductId)
                  .SetProperty(m => m.Quantity, orderItem.Quantity)
                  .SetProperty(m => m.UnitPrice, orderItem.UnitPrice)
                  .SetProperty(m => m.CreatedAt, orderItem.CreatedAt)
                  .SetProperty(m => m.CreatedBy, orderItem.CreatedBy)
                  .SetProperty(m => m.UpdatedAt, orderItem.UpdatedAt)
                  .SetProperty(m => m.UpdatedBy, orderItem.UpdatedBy)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateOrderItem")
        .WithOpenApi();

        group.MapPost("/", async (OrderItem orderItem, AppDbContext db) =>
        {
            db.OrderItems.Add(orderItem);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/OrderItem/{orderItem.Id}",orderItem);
        })
        .WithName("CreateOrderItem")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.OrderItems
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteOrderItem")
        .WithOpenApi();
    }
}}