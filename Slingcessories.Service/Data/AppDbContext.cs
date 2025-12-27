using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Models;

namespace Slingcessories.Service.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subcategory> Subcategories => Set<Subcategory>();
    public DbSet<Accessory> Accessories => Set<Accessory>();
    public DbSet<Slingshot> Slingshots => Set<Slingshot>();
    public DbSet<AccessorySlingshot> AccessorySlingshots => Set<AccessorySlingshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Slingshot belongs to User
        modelBuilder.Entity<Slingshot>()
            .HasOne(s => s.User)
            .WithMany(u => u.Slingshots)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Categories are shared - no User relationship
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        // Subcategories are shared - no User relationship
        modelBuilder.Entity<Subcategory>()
            .HasIndex(sc => new { sc.CategoryId, sc.Name })
            .IsUnique();

        modelBuilder.Entity<Subcategory>()
            .HasOne(sc => sc.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(sc => sc.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Accessories are shared - no User relationship
        modelBuilder.Entity<Accessory>()
            .Property(a => a.Price)
            .HasPrecision(18, 2);

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

        // Configure many-to-many relationship between Accessory and Slingshot
        modelBuilder.Entity<AccessorySlingshot>()
            .HasKey(a_s => new { a_s.AccessoryId, a_s.SlingshotId });

        modelBuilder.Entity<AccessorySlingshot>()
            .HasOne(a_s => a_s.Accessory)
            .WithMany(a => a.AccessorySlingshots)
            .HasForeignKey(a_s => a_s.AccessoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AccessorySlingshot>()
            .HasOne(a_s => a_s.Slingshot)
            .WithMany(s => s.AccessorySlingshots)
            .HasForeignKey(a_s => a_s.SlingshotId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Slingshot>()
            .HasIndex(s => new { s.Year, s.Model, s.Color })
            .IsUnique();
    }
}
