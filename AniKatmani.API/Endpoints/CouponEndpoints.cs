using AniKatmani.API.Dtos;
using AniKatmani.Business;
using AniKatmani.Entities;
using System.Security.Claims;

namespace AniKatmani.API.Endpoints;

public static class CouponEndpoints
{
    public static void MapCouponEndpoints(this IEndpointRouteBuilder app)
    {
        // Admin: Yeni kupon oluştur
        app.MapPost("/coupons", async (CreateCouponDto dto, CouponService service) =>
        {
            var newCoupon = new Coupon
            {
                Code = dto.Code,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinimumCartAmount = dto.MinimumCartAmount,
                ExpiryDate = dto.ExpiryDate
            };

            try
            {
                var created = await service.CreateCouponAsync(newCoupon);
                return Results.Created($"/coupons/{created.Id}", created);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Checkout: Kupon doğrulama (kullanıcı kodu girdiğinde)
        app.MapPost("/coupons/validate", async (ValidateCouponDto dto, CouponService service, HttpContext httpContext) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);

            var (isValid, message, coupon) = await service.ValidateCouponAsync(dto.Code, userId, dto.TotalPrice);

            if (!isValid)
            {
                return Results.BadRequest(new { message });
            }

            var discount = service.CalculateDiscount(coupon!, dto.TotalPrice);

            return Results.Ok(new
            {
                message,
                couponId = coupon!.Id,
                discountAmount = discount,
                finalPrice = dto.TotalPrice - discount
            });
        })
        .RequireAuthorization();

        // Admin: Tüm kuponları listele
        app.MapGet("/coupons", async (CouponService service) =>
        {
            var coupons = await service.GetAllCouponsAsync();
            return Results.Ok(coupons);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Admin: Kupon sil
        app.MapDelete("/coupons/{id:int}", async (int id, CouponService service) =>
        {
            var success = await service.DeleteCouponAsync(id);
            if (!success) return Results.NotFound("Kupon bulunamadı.");
            return Results.Ok(new { message = "Kupon silindi." });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}