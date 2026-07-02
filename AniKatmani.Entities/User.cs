using System.ComponentModel.DataAnnotations;
namespace AniKatmani.Entities;

public class User
{
    public int Id { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }
    [Required][MaxLength(255)]  public string PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsAdmin { get; set; } = false;

}
