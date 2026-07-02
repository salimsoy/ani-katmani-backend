using AniKatmani.API.Dtos;
using AniKatmani.Business;
using System.Security.Claims;

namespace AniKatmani.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (CreateOrderDto dto, OrderService orderService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);
            var order = await orderService.CreateOrderAsync(dto.FullName, dto.Address, dto.PhoneNumber, userId);

            if (order == null)
            {
                return Results.BadRequest("Sepetiniz boş, sipariş oluşturulamaz.");
            }

            return Results.Json(new { message = "Siparişiniz alındı!", orderId = order.Id }, statusCode: 201);
        })
        .RequireAuthorization();

        app.MapGet("/orders", async (OrderService orderService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);
            var orders = await orderService.GetAllOrdersAsync(userId);
            return Results.Ok(orders);
        })
        .RequireAuthorization();

        app.MapGet("/orders/admin", async (OrderService orderService) =>
        {
            var orders = await orderService.GetAllOrdersForAdminAsync();
            return Results.Ok(orders);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        app.MapPut("/orders/{id}/status", async (int id, UpdateStatusDto dto, OrderService orderService) =>
        {
            var order = await orderService.UpdateOrderStatusAsync(id, dto.Status);
            if (order == null)
            {
                return Results.NotFound("Sipariş bulunamadı.");
            }
            return Results.Ok(new { message = "sipariş durumu güncellendi.", status = order.Status });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}