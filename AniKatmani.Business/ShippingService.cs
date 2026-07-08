using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;


namespace AniKatmani.Business;

public class ShippingService
{
    private readonly AniKatmaniDbContext _dbContext;

    public ShippingService(AniKatmaniDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShippingOption> CreateShippingOptionAsync(ShippingOption shippingOption)
    {
        var existingOption = await _dbContext.ShippingOptions.FirstOrDefaultAsync(s => s.Name == shippingOption.Name);
        if (existingOption != null)
        {
            throw new InvalidOperationException("Bu isimde bir kargo seçeneği zaten mevcut.");
        }

        _dbContext.ShippingOptions.Add(shippingOption);
        await _dbContext.SaveChangesAsync();
        return shippingOption;
    }

    public async Task<List<ShippingOption>> GetAllShippingOptionsAsync()
    {
        return await _dbContext.ShippingOptions
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ShippingOption>> GetActiveShippingOptionsAsync()
    {
        return await _dbContext.ShippingOptions
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ShippingOption?> UpdateShippingOptionAsync(int id, ShippingOption updatedData)
    {
        var shippingOption = await _dbContext.ShippingOptions.FindAsync(id);
        if (shippingOption == null) return null;

        // Eğer isim değiştiriliyorsa, yeni ismin mevcut olmadığını kontrol et
        if (shippingOption.Name != updatedData.Name)
        {
            var existingOption = await _dbContext.ShippingOptions.FirstOrDefaultAsync(s => s.Name == updatedData.Name);
            if (existingOption != null)
            {
                throw new InvalidOperationException("Bu isimde bir kargo seçeneği zaten mevcut.");
            }
        }

        shippingOption.Name = updatedData.Name;
        shippingOption.Price = updatedData.Price;
        shippingOption.IsActive = updatedData.IsActive;

        await _dbContext.SaveChangesAsync();
        return shippingOption;
    }

    public async Task<bool> DeleteShippingOptionAsync(int id)
    {
        var shippingOption = await _dbContext.ShippingOptions.FindAsync(id);
        if (shippingOption == null) return false;

        _dbContext.ShippingOptions.Remove(shippingOption);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
