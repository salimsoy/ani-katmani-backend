using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniKatmani.Business;
public class FigurineService
{
    private readonly AniKatmaniDbContext _dbContext;

    public FigurineService(AniKatmaniDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Figurine> AddFigurineAsync(Figurine newFigurine)
    {
        _dbContext.Figurines.Add(newFigurine);
        await _dbContext.SaveChangesAsync();
        return newFigurine;
    }

    public async Task<List<Figurine>> GetAllFigurinesAsync()
    {
        return await _dbContext.Figurines.AsNoTracking().ToListAsync();
    }

    public async Task<Figurine?> GetFigurineByIdAsync(int id)
    {
        return await _dbContext.Figurines.FindAsync(id);
    }

    public async Task<Figurine?> UpdateFigurineAsync(int id, Figurine updatedFigurine)
    {
        var figurine = await _dbContext.Figurines.FindAsync(id);
        if (figurine == null) return null;

        figurine.Name = updatedFigurine.Name;
        figurine.Price = updatedFigurine.Price;
        figurine.FilamentType = updatedFigurine.FilamentType;
        figurine.Scale = updatedFigurine.Scale;
        figurine.PrintTimeInHours = updatedFigurine.PrintTimeInHours;
        figurine.ImageUrl = updatedFigurine.ImageUrl;

        await _dbContext.SaveChangesAsync();
        return figurine;
    }

    public async Task<bool> DeleteFigurineAsync(int id)
    {
        var figurine = await _dbContext.Figurines.FindAsync(id);
        if (figurine == null) return false;

        _dbContext.Figurines.Remove(figurine);
        await _dbContext.SaveChangesAsync();
        return true;
    }


}
