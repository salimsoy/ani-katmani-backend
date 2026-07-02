using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniKatmani.Business;

public class OrderService
{
    private readonly AniKatmaniDbContext _dbContext;

    public OrderService(AniKatmaniDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> CreateOrderAsync(string fullName, string address, string phoneNumber, int userId)
    {
        var cartItems = await _dbContext.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Figurine)
            .ToListAsync();

        if (cartItems.Count == 0)
        {
            return null; // Sepet boş, sipariş oluşturulamaz
        }

        var totalPrice = cartItems.Sum(c => c.Figurine!.Price * c.Quantity);

        var order = new Order
        {   
            UserId = userId,
            FullName = fullName,
            Address = address,
            PhoneNumber = phoneNumber,
            TotalPrice = totalPrice,
            Status = "Beklemede",
            OrderItems = cartItems.Select(c => new OrderItem
            {
                FigurineId = c.FigurineId,
                Quantity = c.Quantity,
                UnitPrice = c.Figurine!.Price
            }).ToList()
        };

        _dbContext.Orders.Add(order);
        _dbContext.CartItems.RemoveRange(cartItems);

        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> GetAllOrdersAsync(int userId)
    {
        return await _dbContext.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Figurine)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null) return null;

        order.Status = newStatus;
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> GetAllOrdersForAdminAsync()
    {
        return await _dbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Figurine)
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}