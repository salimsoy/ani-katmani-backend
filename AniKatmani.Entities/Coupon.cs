using System.ComponentModel.DataAnnotations;

namespace AniKatmani.Entities;

public class Coupon
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; }

    [Required]
    public string DiscountType { get; set; } // "Percentage" veya "Fixed"

    public decimal DiscountValue { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal MinimumCartAmount { get; set; } = 0m; // Kuponun geçerli olabilmesi için minimum sipariş tutarı
}