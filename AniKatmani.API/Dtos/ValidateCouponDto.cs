namespace AniKatmani.API.Dtos;

public class ValidateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}