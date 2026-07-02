
namespace AniKatmani.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; } // Siparişi veren kullanıcı
    public string FullName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Pending"; // Örn: Pending, Shipped, Delivered

    // İlişkisel bağ: Bir siparişin birden fazla sipariş öğesi olabilir
    public List<OrderItem> OrderItems { get; set; } = new();

    public User? User { get; set; } // Siparişi veren kullanıcı ile ilişki
}
