using Microsoft.EntityFrameworkCore;
using lengocsiliem_2122110324.Model;

namespace lengocsiliem_2122110324.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
