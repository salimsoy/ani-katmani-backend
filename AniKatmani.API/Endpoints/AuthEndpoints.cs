using AniKatmani.API.Dtos;
using AniKatmani.Business;
using AniKatmani.Business.Dto;


namespace AniKatmani.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterDto dto, AuthService authService) =>
        {
            var result = await authService.RegisterUserAsync(dto);
            return result ? Results.Ok() : Results.BadRequest();
        });

        app.MapPost("/auth/login", async (LoginDto dto, AuthService authService) =>
        {
            var loginResult = await authService.LoginUserAsync(dto.Email, dto.Password);
            return loginResult == null ? Results.Unauthorized() : Results.Ok(loginResult);
        });
    }
}