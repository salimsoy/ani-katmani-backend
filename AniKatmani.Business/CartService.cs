using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniKatmani.Business;

public class CartService
{
    private readonly AniKatmaniDbContext _dbContext;

    public CartService(AniKatmaniDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CartItem>> GetCartItemsAsync(int userId)
    {
        return await _dbContext.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Figurine)
            .ToListAsync();
    }

    public async Task AddToCartAsync(CartItem newItem, int userId)
    {
        newItem.UserId = userId; // Frontend'den gelen UserId'yi yok say, gerçek değeri buraya yaz

        var existingItem = await _dbContext.CartItems
            .FirstOrDefaultAsync(c => c.FigurineId == newItem.FigurineId && c.UserId == userId);

        if (existingItem != null)
        {
            existingItem.Quantity += newItem.Quantity;
        }
        else
        {
            _dbContext.CartItems.Add(newItem);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateQuantityAsync(int id, int newQuantity)
    {
        var cartItem = await _dbContext.CartItems.FindAsync(id);
        if (cartItem == null) return false;

        if (newQuantity <= 0)
        {
            _dbContext.CartItems.Remove(cartItem);
        }
        else
        {
            cartItem.Quantity = newQuantity;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFromCartAsync(int id)
    {
        var cartItem = await _dbContext.CartItems.FindAsync(id);
        if (cartItem == null) return false;

        _dbContext.CartItems.Remove(cartItem);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}