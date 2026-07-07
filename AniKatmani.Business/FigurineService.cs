using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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

    public async Task<(List<Figurine> Items, int TotalCount)> GetFilteredFigurinesAsync(
        string? search,
        string? filamentType,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy,
        int page,
        int pageSize)
    {
        var query = _dbContext.Figurines.AsQueryable();

        // 1. Arama filtresi (isim veya filament tipinde geçiyor mu)
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(f => f.Name.Contains(search) || f.FilamentType.Contains(search));
        }

        // 2. Filament tipi filtresi
        if (!string.IsNullOrEmpty(filamentType) && filamentType != "Tümü")
        {
            query = query.Where(f => f.FilamentType == filamentType);
        }

        // 3. Fiyat aralığı filtresi
        if (minPrice.HasValue)
        {
            query = query.Where(f => f.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(f => f.Price <= maxPrice.Value);
        }

        // 4. TotalCount - filtrelerden SONRA, sayfalamadan ÖNCE sayılıyor
        var totalCount = await query.CountAsync();

        // 5. Sıralama
        query = sortBy switch
        {
            "priceAsc" => query.OrderBy(f => f.Price),
            "priceDesc" => query.OrderByDescending(f => f.Price),
            "nameAsc" => query.OrderBy(f => f.Name),
            _ => query.OrderBy(f => f.Id)
        };

        // 6. Sayfalama (Skip/Take)
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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

    public async Task<bool> DeleteFigurineAsync(int id, string webRootPath)
    {
        var figurine = await _dbContext.Figurines.FindAsync(id);
        if (figurine == null) return false;
        if (!string.IsNullOrEmpty(figurine.ImageUrl))
        {
            string fileName = Path.GetFileName(figurine.ImageUrl);
            var filePath = Path.Combine(webRootPath, "uploads", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        _dbContext.Figurines.Remove(figurine);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<string> UploadImageAsync(IFormFile file, string webRootPath, string baseUrl)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Sadece jpg, jpeg, png dosyaları yüklenebilir.");
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadsPath = Path.Combine(webRootPath, "uploads");
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"{baseUrl}/uploads/{fileName}";
    }


}
