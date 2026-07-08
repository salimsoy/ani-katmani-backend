
namespace AniKatmani.API.Dtos;

public class CreateGuestOrderDto
{
    public string FullName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; } // Misafir siparişlerinde iletişim için
    public int ShippingOptionId { get; set; }
    public List<GuestCartItemDto> CartItems { get; set; } = new();
}
