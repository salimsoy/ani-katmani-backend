namespace AniKatmani.Entities;

public class CartItem
{
    public int Id { get; set; }

    public int UserId { get; set; } // Sepeti oluşturan kullanıcı
    
    public int FigurineId { get; set; }
    
    public int Quantity { get; set; }

    public Figurine? Figurine { get; set; }
}