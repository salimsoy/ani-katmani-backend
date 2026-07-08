
namespace AniKatmani.API.Dtos;

public class UpdateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty; // "Percentage" veya "Fixed"
    public decimal DiscountValue { get; set; }
    public decimal MinimumCartAmount { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }

}
