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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map Product entity to lowercase table "products"
            modelBuilder.Entity<Product>().ToTable("products");

            // Optional: map all column names to lowercase for PostgreSQL
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.Qty).HasColumnName("qty");
                entity.Property(e => e.ImageUrl).HasColumnName("imageurl");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}