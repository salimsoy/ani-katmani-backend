
namespace AniKatmani.Entities;

public class Order
{
    public int Id { get; set; }
    public int? UserId { get; set; } // Siparişi veren kullanıcı
    public string? Email { get; set; } // Misafir siparişlerinde iletişim için (üye siparişlerinde boş kalabilir)
    public string FullName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Pending"; // Örn: Pending, Shipped, Delivered

    // İlişkisel bağ: Bir siparişin birden fazla sipariş öğesi olabilir
    public List<OrderItem> OrderItems { get; set; } = new();

    public User? User { get; set; } // Siparişi veren kullanıcı ile ilişki

    public int? CouponId { get; set; } // Kullanılan kuponun ID'si (varsa)

    public Coupon? Coupon { get; set; } // Kullanılan kupon ile ilişki

    public decimal DiscountAmount { get; set; } = 0m; // Kupon indirimi miktarı (varsa)

    public int? ShippingOptionId { get; set; } // Seçilen kargo seçeneğinin ID'si (varsa)

    public ShippingOption? ShippingOption { get; set; } // Seçilen kargo seçeneği ile ilişki
    public decimal ShippingCost { get; set; } = 0m; // Kargo ücreti (varsa)
}
