using Microsoft.EntityFrameworkCore;
using Slingsessory.service.Models;

namespace Slingsessory.service.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subcategory> Subcategories => Set<Subcategory>();
    public DbSet<Accessory> Accessories => Set<Accessory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Subcategory>()
            .HasIndex(sc => new { sc.CategoryId, sc.Name })
            .IsUnique();

        modelBuilder.Entity<Subcategory>()
            .HasOne(sc => sc.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(sc => sc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Accessory>()
            .HasOne(a => a.Category)
            .WithMany()
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Accessory>()
            .HasOne(a => a.Subcategory)
            .WithMany(sc => sc.Accessories)
            .HasForeignKey(a => a.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
