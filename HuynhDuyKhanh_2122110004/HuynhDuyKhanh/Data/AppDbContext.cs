using Microsoft.EntityFrameworkCore;
using HuynhDuyKhanh.Model;  // Ensure this is the correct namespace for your models

namespace HuynhDuyKhanh.Data  // Correct namespace
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mối quan hệ giữa Product và Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category) // Mỗi Product có một Category
                .WithMany(c => c.Products) // Mỗi Category có nhiều Products
                .HasForeignKey(p => p.Cat_Id); // Cat_Id trong Product là khóa ngoại

            // Mối quan hệ giữa Order và User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)  // Mỗi Order có một User
                .WithMany(u => u.Orders)  // Mỗi User có nhiều Orders
                .HasForeignKey(o => o.UserId); // UserId trong Order là khóa ngoại

            // Mối quan hệ giữa Order và OrderDetail
            // Uncomment and complete this if needed.
        }
    }
}
