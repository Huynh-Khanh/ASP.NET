using HuynhDuyKhanh_2122110004.Data;
using HuynhDuyKhanh_2122110004.Model;
using Microsoft.EntityFrameworkCore;


namespace HuynhDuyKhanh_2122110004.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}