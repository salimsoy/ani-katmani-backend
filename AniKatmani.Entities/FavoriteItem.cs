namespace AniKatmani.Entities;

public class FavoriteItem
{
    public int Id { get; set; }

    // Hangi kullanıcının beğendiğini tutmak için (İleride çok işimize yarayacak)
    public int UserId { get; set; }

    // Hangi ürünü (figürü) beğendiği
    public int FigurineId { get; set; }

    // Entity Framework'ün INNER JOIN (Include) yapabilmesi için ilişki tanımı
    public Figurine? Figurine { get; set; }

    // Ürünün ne zaman beğenildiği (Arayüzde en son beğenilenleri en üstte göstermek için)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}