
using AniKatmani.API.Dtos;
using AniKatmani.Business;
using AniKatmani.Entities;
using System.Security.Claims;

namespace AniKatmani.API.Endpoints;

public static class ShippingEndpoints
{
    public static void MapShippingEndpoints(this IEndpointRouteBuilder app)
    {
        // Admin: Yeni kargo seçeneği oluştur
        app.MapPost("/shipping-options", async (CreateShippingOptionDto dto, ShippingService service) =>
        {
            var newShippingOption = new ShippingOption
            {
                Name = dto.Name,
                Price = dto.Price
            };

            try
            {
                var created = await service.CreateShippingOptionAsync(newShippingOption);
                return Results.Created($"/shipping-options/{created.Id}", created);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Admin: Tüm kargo seçeneklerini listele
        app.MapGet("/shipping-options", async (ShippingService service) =>
        {
            var shippingOptions = await service.GetAllShippingOptionsAsync();
            return Results.Ok(shippingOptions);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Checkout: Aktif kargo seçeneklerini listele
        app.MapGet("/shipping-options/active", async (ShippingService service) =>
        {
            var activeShippingOptions = await service.GetActiveShippingOptionsAsync();
            return Results.Ok(activeShippingOptions);
        });
        
        // Admin: Kargo seçeneğini güncelle
        app.MapPut("/shipping-options/{id:int}", async (int id, UpdateShippingOptionDto dto, ShippingService service) =>
        {
            var updatedData = new ShippingOption
            {
                Name = dto.Name,
                Price = dto.Price,
                IsActive = dto.IsActive
            };

            try
            {
                var updatedOption = await service.UpdateShippingOptionAsync(id, updatedData);
                if (updatedOption == null)
                {
                    return Results.NotFound($"Kargo seçeneği {id} bulunamadı.");
                }
                return Results.Ok(updatedOption);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Admin: Kargo seçeneğini sil
        app.MapDelete("/shipping-options/{id:int}", async (int id, ShippingService service) =>
        {
            var success = await service.DeleteShippingOptionAsync(id);
            if (!success)
            {
                return Results.NotFound($"Kargo seçeneği {id} bulunamadı.");
            }
            return Results.Ok(new { message = "Kargo seçeneği silindi." });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}
