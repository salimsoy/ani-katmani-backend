using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniKatmani.Business;

public class OrderService
{
    private readonly AniKatmaniDbContext _dbContext;
    private readonly CouponService _couponService;

    public OrderService(AniKatmaniDbContext dbContext, CouponService couponService)
    {
        _dbContext = dbContext;
        _couponService = couponService;
    }

    public async Task<Order?> CreateOrderAsync(string fullName, string address, string phoneNumber, int userId, string? couponCode)
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

        int? couponId = null;
        decimal discountAmount = 0m;

        if(!string.IsNullOrEmpty(couponCode))
        {
            var (isValid, message, coupon) = await _couponService.ValidateCouponAsync(couponCode, userId, totalPrice);

            if (!isValid)
            {
                throw new InvalidOperationException(message);
            }
            couponId = coupon!.Id;
            discountAmount = _couponService.CalculateDiscount(coupon!, totalPrice);
            totalPrice -= discountAmount;
        }

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
            }).ToList(),
            CouponId = couponId,
            DiscountAmount = discountAmount
        };

        _dbContext.Orders.Add(order);
        _dbContext.CartItems.RemoveRange(cartItems);

        if (couponId.HasValue)
        {
            _dbContext.CouponUsages.Add(new CouponUsage
        {
            CouponId = couponId.Value,
            UserId = userId
        });
        }

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