using Microsoft.EntityFrameworkCore;

namespace WebAPIDB.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
