using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Data;

public class AppDbContextTests
{
    [Fact]
    public async Task DbContext_CanCreateAndQueryUsers()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "test", FirstName = "John", LastName = "Doe", Email = "john@test.com" };

        // Act
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var retrieved = await db.Users.FindAsync("test");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task DbContext_EnforcesUniqueEmailConstraint()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        db.Users.AddRange(
            new User { Id = "1", FirstName = "John", LastName = "Doe", Email = "duplicate@test.com" },
            new User { Id = "2", FirstName = "Jane", LastName = "Doe", Email = "duplicate@test.com" }
        );

        // Act & Assert
        // Note: In-memory database doesn't enforce unique constraints, but we test that the index is configured
        var userEntity = db.Model.FindEntityType(typeof(User));
        var emailIndex = userEntity!.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));
        
        emailIndex.Should().NotBeNull();
        emailIndex!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public async Task DbContext_CanCreateAndQueryCategories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };

        // Act
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        var retrieved = await db.Categories.FindAsync(category.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("TestCategory");
    }

    [Fact]
    public void DbContext_ConfiguresUniqueCategoryName()
    {
        // Arrange
        using var db = TestHelpers.CreateInMemoryDbContext();

        // Act
        var categoryEntity = db.Model.FindEntityType(typeof(Category));
        var nameIndex = categoryEntity!.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "Name"));

        // Assert
        nameIndex.Should().NotBeNull();
        nameIndex!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public async Task DbContext_CanCreateSubcategoryWithCategory()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var subcategory = new Subcategory { Name = "TestSubcategory", CategoryId = category.Id };

        // Act
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        // Assert
        var retrieved = await db.Subcategories
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == subcategory.Id);
        
        retrieved.Should().NotBeNull();
        retrieved!.Category.Should().NotBeNull();
        retrieved.Category!.Name.Should().Be("TestCategory");
    }

    [Fact]
    public void DbContext_ConfiguresUniqueSubcategoryPerCategory()
    {
        // Arrange
        using var db = TestHelpers.CreateInMemoryDbContext();

        // Act
        var subcategoryEntity = db.Model.FindEntityType(typeof(Subcategory));
        var compositeIndex = subcategoryEntity!.GetIndexes()
            .FirstOrDefault(i => i.Properties.Count == 2 && 
                                 i.Properties.Any(p => p.Name == "CategoryId") &&
                                 i.Properties.Any(p => p.Name == "Name"));

        // Assert
        compositeIndex.Should().NotBeNull();
        compositeIndex!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public async Task DbContext_CanCreateAccessoryWithRelationships()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        var subcategory = new Subcategory { Name = "TestSubcategory", CategoryId = 1 };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        subcategory.CategoryId = category.Id;
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var accessory = new Accessory 
        { 
            Title = "TestAccessory",
            Price = 99.99m,
            CategoryId = category.Id,
            SubcategoryId = subcategory.Id,
            Wishlist = false
        };

        // Act
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        // Assert
        var retrieved = await db.Accessories
            .Include(a => a.Category)
            .Include(a => a.Subcategory)
            .FirstOrDefaultAsync(a => a.Id == accessory.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Category.Should().NotBeNull();
        retrieved.Subcategory.Should().NotBeNull();
    }

    [Fact]
    public void DbContext_ConfiguresPricePrecision()
    {
        // Arrange
        using var db = TestHelpers.CreateInMemoryDbContext();

        // Act
        var accessoryEntity = db.Model.FindEntityType(typeof(Accessory));
        var priceProperty = accessoryEntity!.FindProperty("Price");

        // Assert
        priceProperty.Should().NotBeNull();
        priceProperty!.GetPrecision().Should().Be(18);
        priceProperty.GetScale().Should().Be(2);
    }

    [Fact]
    public async Task DbContext_CanCreateSlingshotWithUser()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };

        // Act
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        // Assert
        var retrieved = await db.Slingshots
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == slingshot.Id);

        retrieved.Should().NotBeNull();
        retrieved!.User.Should().NotBeNull();
        retrieved.User!.Email.Should().Be("test@test.com");
    }

    [Fact]
    public void DbContext_ConfiguresUniqueSlingshotComposite()
    {
        // Arrange
        using var db = TestHelpers.CreateInMemoryDbContext();

        // Act
        var slingshotEntity = db.Model.FindEntityType(typeof(Slingshot));
        var compositeIndex = slingshotEntity!.GetIndexes()
            .FirstOrDefault(i => i.Properties.Count == 3 &&
                                 i.Properties.Any(p => p.Name == "Year") &&
                                 i.Properties.Any(p => p.Name == "Model") &&
                                 i.Properties.Any(p => p.Name == "Color"));

        // Assert
        compositeIndex.Should().NotBeNull();
        compositeIndex!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public async Task DbContext_CanCreateAccessorySlingshotRelationship()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        var category = new Category { Name = "TestCategory" };
        db.Users.Add(user);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        var accessory = new Accessory { Title = "TestAccessory", Price = 10, CategoryId = category.Id, Wishlist = false };
        db.Slingshots.Add(slingshot);
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        var relationship = new AccessorySlingshot 
        { 
            AccessoryId = accessory.Id, 
            SlingshotId = slingshot.Id,
            Quantity = 5
        };

        // Act
        db.AccessorySlingshots.Add(relationship);
        await db.SaveChangesAsync();

        // Assert
        var retrieved = await db.AccessorySlingshots
            .Include(a_s => a_s.Accessory)
            .Include(a_s => a_s.Slingshot)
            .FirstOrDefaultAsync(a_s => a_s.AccessoryId == accessory.Id && a_s.SlingshotId == slingshot.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Quantity.Should().Be(5);
        retrieved.Accessory.Should().NotBeNull();
        retrieved.Slingshot.Should().NotBeNull();
    }

    [Fact]
    public void DbContext_ConfiguresAccessorySlingshotCompositeKey()
    {
        // Arrange
        using var db = TestHelpers.CreateInMemoryDbContext();

        // Act
        var entity = db.Model.FindEntityType(typeof(AccessorySlingshot));
        var key = entity!.FindPrimaryKey();

        // Assert
        key.Should().NotBeNull();
        key!.Properties.Should().HaveCount(2);
        key.Properties.Select(p => p.Name).Should().Contain("AccessoryId");
        key.Properties.Select(p => p.Name).Should().Contain("SlingshotId");
    }

    [Fact]
    public async Task DbContext_CascadeDeletesAccessorySlingshotWhenAccessoryDeleted()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        var category = new Category { Name = "TestCategory" };
        db.Users.Add(user);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        var accessory = new Accessory { Title = "TestAccessory", Price = 10, CategoryId = category.Id, Wishlist = false };
        db.Slingshots.Add(slingshot);
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        db.AccessorySlingshots.Add(new AccessorySlingshot 
        { 
            AccessoryId = accessory.Id, 
            SlingshotId = slingshot.Id,
            Quantity = 1
        });
        await db.SaveChangesAsync();

        // Act
        db.Accessories.Remove(accessory);
        await db.SaveChangesAsync();

        // Assert
        var relationship = await db.AccessorySlingshots
            .FirstOrDefaultAsync(a_s => a_s.AccessoryId == accessory.Id);
        relationship.Should().BeNull();
    }

    [Fact]
    public void DbContext_HasAllRequiredDbSets()
    {
        // Arrange & Act
        using var db = TestHelpers.CreateInMemoryDbContext();

        // Assert
        db.Users.Should().NotBeNull();
        db.Categories.Should().NotBeNull();
        db.Subcategories.Should().NotBeNull();
        db.Accessories.Should().NotBeNull();
        db.Slingshots.Should().NotBeNull();
        db.AccessorySlingshots.Should().NotBeNull();
    }
}
