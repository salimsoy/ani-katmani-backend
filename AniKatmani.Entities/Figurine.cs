namespace AniKatmani.Entities;

public class Figurine
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // Ürünlerimize özel detaylar
    public string FilamentType { get; set; } // Örn: PLA, Resin
    public string Scale { get; set; } // Örn: 1/10, 1/6
    public int PrintTimeInHours { get; set; } // Basım süresi

    public string? ImageUrl { get; set; }
        
}
