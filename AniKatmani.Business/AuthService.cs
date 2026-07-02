using AniKatmani.DataAccess;
using AniKatmani.Entities;
using AniKatmani.Business.Dto;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace AniKatmani.Business;

public class AuthService
{
    private readonly AniKatmaniDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(AniKatmaniDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }
    public async Task<bool> RegisterUserAsync(RegisterDto dto)
    {
        var existingItem = await _dbContext.Users
          .FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingItem != null)
        {
            return false; // Kullanıcı zaten mevcut
        }
        var newUser = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) // Şifreyi hash'le
        };

        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
        return true; // Başarılı kayıt durumunu döndür
    }

    public async Task<LoginResponseDto?> LoginUserAsync(string email, string password)
    {
        // 1. Email ile kullanıcıyı bul
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null; // Kullanıcı yok
        }

        // 2. Şifre doğru mu kontrol et (BCrypt.Verify)
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null; // Şifre yanlış
        }


        // 3. Her şey doğruysa, kullanıcıyı dön
        return new LoginResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Email = user.Email,
            Token = GenerateJwtToken(user),
            IsAdmin = user.IsAdmin
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
