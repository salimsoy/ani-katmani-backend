using AniKatmani.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniKatmani.DataAccess;

public class AniKatmaniDbContext : DbContext
{
        public AniKatmaniDbContext(DbContextOptions<AniKatmaniDbContext> options) : base(options)
        {

        }
        public DbSet<Figurine> Figurines { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FavoriteItem> FavoriteItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        public DbSet<ShippingOption> ShippingOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<User>()
                        .HasIndex(u => u.Email)
                        .IsUnique();
                
                modelBuilder.Entity<Coupon>()
                        .HasIndex(c => c.Code)
                        .IsUnique();
                
                modelBuilder.Entity<User>()
                        .ToTable("Users");
        
        }

        
}
