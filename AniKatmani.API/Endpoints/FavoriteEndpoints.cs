using AniKatmani.API.Dtos;
using AniKatmani.Business;
using System.Security.Claims;

namespace AniKatmani.API.Endpoints;

public static class FavoriteEndpoints
{
    public static void MapFavoriteEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/favorites", async (AddFavoriteDto dto, FavoriteService favoriteService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);
            var result = await favoriteService.AddFavoriteAsync(userId, dto.FigurineId);
            return Results.Ok(new { message = "Ürün favorilere eklendi." });
        })
        .RequireAuthorization();

        app.MapGet("/favorites", async (FavoriteService favoriteService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);
            var favorites = await favoriteService.GetFavoritesAsync(userId);
            return Results.Ok(favorites);
        })
        .RequireAuthorization();

        app.MapDelete("/favorites/{figurineId}", async (int figurineId, FavoriteService favoriteService, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);
            var result = await favoriteService.RemoveFavoriteAsync(userId, figurineId);
            if (!result) return Results.NotFound("Favori bulunamadı.");
            return Results.Ok(new { message = "Ürün favorilerden çıkarıldı." });
        })
        .RequireAuthorization();
    }
}