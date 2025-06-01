using AecciWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AecciWeb.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración explícita de precisión para el campo Price
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Opcional: configurar CreatedAt/UpdatedAt con valores por defecto
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            modelBuilder.Entity<Product>()
                .Property(p => p.UpdatedAt)
                .IsRequired(false);

            // Longitudes y requisitos ya los pusimos con DataAnnotations,
            // pero aquí se podrían reforzar, ej:
            // modelBuilder.Entity<Product>()
            //     .Property(p => p.Name)
            //     .HasMaxLength(100)
            //     .IsRequired();

        }
    }
}
