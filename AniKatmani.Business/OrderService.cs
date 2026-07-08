using AniKatmani.DataAccess;
using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;
using AniKatmani.Business.Dto;

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

    public async Task<Order?> CreateOrderAsync(string fullName, string address, string phoneNumber, int userId, string? couponCode, int shippingOptionId)
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

        var selectedShippingOption = await _dbContext.ShippingOptions
            .FirstOrDefaultAsync(s => s.Id == shippingOptionId && s.IsActive);
        if (selectedShippingOption == null)
        {
            throw new InvalidOperationException("Seçilen kargo seçeneği geçerli değil.");
        }
        totalPrice += selectedShippingOption.Price;

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
            DiscountAmount = discountAmount,
            ShippingOptionId = selectedShippingOption.Id,
            ShippingCost = selectedShippingOption.Price

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

    public async Task<Order?> GetOrderByIdAsync(int orderId, int userId)
    {
        var order = await _dbContext.Orders
            .Where(o => o.Id == orderId && o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Figurine)
            .FirstOrDefaultAsync();

        return order;
    }

    public async Task<Order?> CreateGuestOrderAsync(string fullName, string address, string phoneNumber, string email, int shippingOptionId, List<GuestCartItemDto> cartItems)
    {
        if (cartItems.Count == 0)
        {
            return null; // Sepet boş, sipariş oluşturulamaz
        }

        var cartItemFigurineIds = cartItems.Select(c => c.FigurineId).ToList();
        var figurines = await _dbContext.Figurines.Where(f => cartItemFigurineIds.Contains(f.Id)).ToListAsync();
        if (figurines.Count != cartItemFigurineIds.Count)
        {
            throw new InvalidOperationException("Sepetteki bazı figürinler geçerli değil.");
        }

        var totalPrice = cartItems.Sum(c => figurines.First(f => f.Id == c.FigurineId).Price * c.Quantity);

        var selectedShippingOption = await _dbContext.ShippingOptions
            .FirstOrDefaultAsync(s => s.Id == shippingOptionId && s.IsActive);
        if (selectedShippingOption == null)
        {
            throw new InvalidOperationException("Seçilen kargo seçeneği geçerli değil.");
        }
        totalPrice += selectedShippingOption.Price;

        var order = new Order
        {
            FullName = fullName,
            Address = address,
            PhoneNumber = phoneNumber,
            Email = email,
            TotalPrice = totalPrice,
            Status = "Beklemede",
            OrderItems = cartItems.Select(c => new OrderItem
            {
                FigurineId = c.FigurineId,
                Quantity = c.Quantity,
                UnitPrice = figurines.First(f => f.Id == c.FigurineId).Price
            }).ToList(),
            ShippingOptionId = selectedShippingOption.Id,
            ShippingCost = selectedShippingOption.Price
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        return order;
    } 
}
