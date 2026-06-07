using Microsoft.EntityFrameworkCore;
using ProductApp.Models;

namespace ProductApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=products.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Электроника" },
                new Category { Id = 2, Name = "Одежда" },
                new Category { Id = 3, Name = "Книги" },
                new Category { Id = 4, Name = "Дом и сад" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, CategoryId = 1, Name = "Смартфон", Price = 25000 },
                new Product { Id = 2, CategoryId = 1, Name = "Ноутбук", Price = 55000 },
                new Product { Id = 3, CategoryId = 1, Name = "Наушники", Price = 3000 },
                new Product { Id = 4, CategoryId = 2, Name = "Футболка", Price = 1200 },
                new Product { Id = 5, CategoryId = 2, Name = "Джинсы", Price = 3500 },
                new Product { Id = 6, CategoryId = 2, Name = "Куртка", Price = 8500 },
                new Product { Id = 7, CategoryId = 3, Name = "Война и мир", Price = 800 },
                new Product { Id = 8, CategoryId = 3, Name = "1984", Price = 450 },
                new Product { Id = 9, CategoryId = 3, Name = "Мастер и Маргарита", Price = 600 },
                new Product { Id = 10, CategoryId = 4, Name = "Диван", Price = 45000 },
                new Product { Id = 11, CategoryId = 4, Name = "Стул", Price = 5000 },
                new Product { Id = 12, CategoryId = 4, Name = "Лампа", Price = 2500 }
            );
        }
    }
}