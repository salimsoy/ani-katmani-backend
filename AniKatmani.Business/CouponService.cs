using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniKatmani.Business;

public class CouponService
{
    private readonly AniKatmaniDbContext _dbContext;

    public CouponService(AniKatmaniDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Geriye sadece kuponu değil, (Başarılı mı?, Mesaj, Kupon) üçlüsünü dönüyoruz
    // string userId yerine int userId yapıyoruz
    public async Task<(bool IsValid, string Message, Coupon? Coupon)> ValidateCouponAsync(string code, int userId, decimal totalPrice)
    {
        var coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Code == code && c.IsActive);

        if (coupon == null)
            return (false, "Geçersiz veya süresi dolmuş bir kupon kodu girdiniz.", null);

        // Tarih kontrolü
        if (coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.UtcNow)
            return (false, "Bu kuponun kullanım süresi dolmuş.", null);

        // Kullanım kontrolü
        var usage = await _dbContext.CouponUsages.FirstOrDefaultAsync(u => u.CouponId == coupon.Id && u.UserId == userId);
        if (usage != null)
            return (false, "Bu kuponu daha önce kullandınız.", null);

        // Alt limit kontrolü
        if (coupon.MinimumCartAmount > totalPrice)
            return (false, $"Bu kuponu kullanmak için sepet tutarınız en az {coupon.MinimumCartAmount} TL olmalıdır.", null);

        return (true, "Kupon başarıyla uygulandı!", coupon);
    }

    public decimal CalculateDiscount(Coupon coupon, decimal totalPrice)
    {
        if (coupon.DiscountType == "Percentage")
        {
            var discount = totalPrice * (coupon.DiscountValue / 100);
            return Math.Min(discount, totalPrice);
        }
        else if (coupon.DiscountType == "Fixed")
        {
            // Sepet tutarından fazlasını indirim olarak düşmesini engeller
            return Math.Min(coupon.DiscountValue, totalPrice); 
        }

        return 0m;
    }

    public async Task<Coupon> CreateCouponAsync(Coupon coupon)
    {
        var existingCoupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Code == coupon.Code);
        if (existingCoupon != null)
        {
            throw new InvalidOperationException("Bu kupon kodu zaten mevcut.");
        }

        _dbContext.Coupons.Add(coupon);
        await _dbContext.SaveChangesAsync();
        return coupon;
    }

    public async Task<List<Coupon>> GetAllCouponsAsync()
    {
        return await _dbContext.Coupons
        .OrderByDescending(c => c.CreatedAt)
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<bool> DeleteCouponAsync(int id)
    {
        var coupon = await _dbContext.Coupons.FindAsync(id);
        if (coupon == null) return false;

        _dbContext.Coupons.Remove(coupon);
        await _dbContext.SaveChangesAsync();
        return true;
        
    }

    public async Task<Coupon?> UpdateCouponAsync(int id, Coupon updatedData)
    {   
        
        var coupon = await _dbContext.Coupons.FindAsync(id);
        if (coupon == null) return null;

        // Kupon kodu değiştiriliyorsa, yeni kodun mevcut olmadığını kontrol et
        if (coupon.Code != updatedData.Code)
        {
            var existingCoupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Code == updatedData.Code);
            if (existingCoupon != null)
            {
                throw new InvalidOperationException("Bu kupon kodu zaten mevcut.");
            }
        }

        coupon.Code = updatedData.Code;
        coupon.DiscountType = updatedData.DiscountType;
        coupon.DiscountValue = updatedData.DiscountValue;
        coupon.MinimumCartAmount = updatedData.MinimumCartAmount;
        coupon.IsActive = updatedData.IsActive;
        coupon.ExpiryDate = updatedData.ExpiryDate;

        await _dbContext.SaveChangesAsync();
        return coupon;
    }
}