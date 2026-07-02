using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AniKatmani.Business;
using AniKatmani.Entities;

namespace AniKatmani.API.Endpoints;

public static class FigurinesEndpoints
{
    public static void MapFigurinesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/figurines").WithTags("Figurines");

        // GET /figurines — herkese açık
        group.MapGet("/", async (FigurineService service) =>
        {
            var figurines = await service.GetAllFigurinesAsync();
            return Results.Ok(figurines);
        });

        // GET /figurines/{id} — herkese açık
        group.MapGet("/{id:int}", async (int id, FigurineService service) =>
        {
            var figurine = await service.GetFigurineByIdAsync(id);
            if (figurine is null) return Results.NotFound("Figür bulunamadı.");
            return Results.Ok(figurine);
        });

        // POST /figurines — sadece admin
        group.MapPost("/", async (Figurine newFigurine, FigurineService service) =>
        {
            var createdFigurine = await service.AddFigurineAsync(newFigurine);
            return Results.Created($"/figurines/{createdFigurine.Id}", createdFigurine);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // PUT /figurines/{id} — sadece admin
        group.MapPut("/{id:int}", async (int id, Figurine updatedFigurine, FigurineService service) =>
        {
            var figurine = await service.UpdateFigurineAsync(id, updatedFigurine);
            if (figurine is null) return Results.NotFound("Figür bulunamadı.");
            return Results.Ok(figurine);
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // DELETE /figurines/{id} — sadece admin
        group.MapDelete("/{id:int}", async (int id, FigurineService service) =>
        {
            var success = await service.DeleteFigurineAsync(id);
            if (!success) return Results.NotFound("Figür bulunamadı.");
            return Results.Ok(new { message = "Figür silindi." });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}