
namespace AniKatmani.API.Dtos;

public class UpdateShippingOptionDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
