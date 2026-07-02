using AniKatmani.API.Dtos;
using AniKatmani.Business;
using AniKatmani.Entities;
using System.Security.Claims;

namespace AniKatmani.API.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/cart", async (CartService cartService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            var cartItems = await cartService.GetCartItemsAsync(userId);
            return Results.Ok(cartItems);
        });

        app.MapPost("/cart", async (CartItem newItem, CartService cartService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            await cartService.AddToCartAsync(newItem, userId);
            return Results.Json(new { message = "Ürün sepete eklendi!" }, statusCode: 201);
        });

        app.MapPut("/cart/{id}", async (int id, UpdateQuantityDto dto, CartService cartService) =>
        {
            var success = await cartService.UpdateQuantityAsync(id, dto.Quantity);
            if (!success) return Results.NotFound("Ürün sepette bulunamadı.");
            return Results.Ok(new { message = "Sepet güncellendi." });
        });

        app.MapDelete("/cart/{id}", async (int id, CartService cartService) =>
        {
            var success = await cartService.RemoveFromCartAsync(id);
            if (!success) return Results.NotFound("Ürün sepette bulunamadı.");
            return Results.Ok(new { message = "Ürün sepetten kaldırıldı." });
        });
    }
}