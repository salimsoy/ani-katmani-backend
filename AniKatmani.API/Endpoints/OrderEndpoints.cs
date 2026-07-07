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

            try
            {
                var order = await orderService.CreateOrderAsync(dto.FullName, dto.Address, dto.PhoneNumber, userId, dto.CouponCode);

                if (order == null)
                {
                    return Results.BadRequest("Sepetiniz boş, sipariş oluşturulamaz.");
                }

                return Results.Json(new
                {
                    message = "Siparişiniz alındı!",
                    orderId = order.Id,
                    originalPrice = order.TotalPrice + order.DiscountAmount,
                    discountAmount = order.DiscountAmount,
                    totalPrice = order.TotalPrice
                }, statusCode: 201);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
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

        app.MapGet("/orders/{id}", async (int id, OrderService orderService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);
            var order = await orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return Results.NotFound("Sipariş bulunamadı.");
            }

            return Results.Ok(order);
        })
        .RequireAuthorization();
    }
}