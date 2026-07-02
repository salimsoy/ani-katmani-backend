namespace AniKatmani.Business.Dto;

public class LoginResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public bool IsAdmin { get; set; }
}
