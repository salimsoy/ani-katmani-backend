namespace AniKatmani.API.Dtos;

public class CreateOrderDto
{
    public string FullName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string? CouponCode { get; set; }
}