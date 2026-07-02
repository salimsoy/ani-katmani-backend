using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniKatmani.Business;

public class FavoriteService
{
    private readonly AniKatmaniDbContext _dbContext;

    public FavoriteService(AniKatmaniDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FavoriteItem> AddFavoriteAsync(int userId, int figurineId)
    {
        var existingFavorite = await _dbContext.FavoriteItems
            .FirstOrDefaultAsync(f => f.UserId == userId && f.FigurineId == figurineId);

        if (existingFavorite != null)
        {
            return existingFavorite;
        }

        var favorite = new FavoriteItem
        {
            UserId = userId,
            FigurineId = figurineId
        };

        _dbContext.FavoriteItems.Add(favorite);
        await _dbContext.SaveChangesAsync();

        return favorite;
    }

    public async Task<List<FavoriteItem>> GetFavoritesAsync(int userId)
    {
        return await _dbContext.FavoriteItems
            .Where(f => f.UserId == userId)
            .Include(f => f.Figurine)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> RemoveFavoriteAsync(int userId, int figurineId)
    {
        var favorite = await _dbContext.FavoriteItems
            .FirstOrDefaultAsync(f => f.UserId == userId && f.FigurineId == figurineId);

        if (favorite == null)
        {
            return false;
        }

        _dbContext.FavoriteItems.Remove(favorite);
        await _dbContext.SaveChangesAsync();

        return true;
    }

}
