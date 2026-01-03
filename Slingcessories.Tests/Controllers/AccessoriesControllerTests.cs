using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Controllers;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Controllers;

public class AccessoriesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoAccessories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accessories = okResult.Value.Should().BeAssignableTo<IEnumerable<AccessoryDto>>().Subject;
        accessories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnsAllAccessories_OrderedByTitle()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        db.Accessories.AddRange(
            new Accessory { Title = "Zebra", Price = 10, CategoryId = category.Id, Wishlist = false },
            new Accessory { Title = "Apple", Price = 20, CategoryId = category.Id, Wishlist = false },
            new Accessory { Title = "Banana", Price = 15, CategoryId = category.Id, Wishlist = true }
        );
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accessories = okResult.Value.Should().BeAssignableTo<IEnumerable<AccessoryDto>>().Subject.ToList();
        accessories.Should().HaveCount(3);
        accessories[0].Title.Should().Be("Apple");
        accessories[1].Title.Should().Be("Banana");
        accessories[2].Title.Should().Be("Zebra");
    }

    [Fact]
    public async Task GetAll_FiltersWishlist_WhenWishlistTrue()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        db.Accessories.AddRange(
            new Accessory { Title = "WishlistItem", Price = 10, CategoryId = category.Id, Wishlist = true },
            new Accessory { Title = "RegularItem", Price = 20, CategoryId = category.Id, Wishlist = false }
        );
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetAll(true);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accessories = okResult.Value.Should().BeAssignableTo<IEnumerable<AccessoryDto>>().Subject.ToList();
        accessories.Should().HaveCount(1);
        accessories[0].Title.Should().Be("WishlistItem");
    }

    [Fact]
    public async Task GetAll_FiltersWishlist_WhenWishlistFalse()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        db.Accessories.AddRange(
            new Accessory { Title = "WishlistItem", Price = 10, CategoryId = category.Id, Wishlist = true },
            new Accessory { Title = "RegularItem", Price = 20, CategoryId = category.Id, Wishlist = false }
        );
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetAll(false);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accessories = okResult.Value.Should().BeAssignableTo<IEnumerable<AccessoryDto>>().Subject.ToList();
        accessories.Should().HaveCount(1);
        accessories[0].Title.Should().Be("RegularItem");
    }

    [Fact]
    public async Task GetAll_IncludesRelatedData_WithSlingshotQuantities()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        var subcategory = new Subcategory { Name = "TestSubcategory", CategoryId = 1 };
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        
        db.Categories.Add(category);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        
        subcategory.CategoryId = category.Id;
        db.Subcategories.Add(subcategory);
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var accessory = new Accessory 
        { 
            Title = "TestAccessory", 
            Price = 100, 
            CategoryId = category.Id, 
            SubcategoryId = subcategory.Id,
            Wishlist = false
        };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        db.AccessorySlingshots.Add(new AccessorySlingshot 
        { 
            AccessoryId = accessory.Id, 
            SlingshotId = slingshot.Id,
            Quantity = 5
        });
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accessories = okResult.Value.Should().BeAssignableTo<IEnumerable<AccessoryDto>>().Subject.ToList();
        accessories.Should().HaveCount(1);
        var dto = accessories[0];
        dto.CategoryName.Should().Be("TestCategory");
        dto.SubcategoryName.Should().Be("TestSubcategory");
        dto.SlinghotIds.Should().NotBeNull();
        dto.SlinghotIds!.Should().Contain(slingshot.Id);
        dto.SlinghotDescriptions.Should().NotBeNull();
        dto.SlinghotDescriptions!.Should().Contain("2020 Model S (Red)");
        dto.SlinghotQuantities.Should().ContainKey(slingshot.Id);
        dto.SlinghotQuantities[slingshot.Id].Should().Be(5);
    }

    [Fact]
    public async Task GetById_ReturnsAccessory_WhenAccessoryExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var accessory = new Accessory 
        { 
            Title = "TestAccessory", 
            Price = 50, 
            CategoryId = category.Id,
            Wishlist = false
        };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetById(accessory.Id);

        // Assert
        result.Result.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("TestAccessory");
        result.Value.Price.Should().Be(50);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenAccessoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Create_CreatesAccessory_WithoutSlingshots()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);
        var createDto = new CreateAccessoryDto(
            "NewAccessory",
            "http://pic.com",
            99.99m,
            "http://url.com",
            true,
            category.Id,
            null,
            null
        );

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var accessoryDto = createdResult.Value.Should().BeOfType<AccessoryDto>().Subject;
        accessoryDto.Title.Should().Be("NewAccessory");
        accessoryDto.Price.Should().Be(99.99m);
        accessoryDto.Wishlist.Should().BeTrue();

        // Verify saved to database
        var saved = await db.Accessories.FindAsync(accessoryDto.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_CreatesAccessory_WithSlingshotQuantities()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Blue", UserId = "user1" };
        
        db.Categories.Add(category);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);
        var quantities = new Dictionary<int, int> { { slingshot.Id, 3 } };
        var createDto = new CreateAccessoryDto(
            "AccessoryWithSlingshot",
            null,
            50m,
            null,
            false,
            category.Id,
            null,
            quantities
        );

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var accessoryDto = createdResult.Value.Should().BeOfType<AccessoryDto>().Subject;
        accessoryDto.SlinghotIds.Should().NotBeNull();
        accessoryDto.SlinghotIds!.Should().Contain(slingshot.Id);
        accessoryDto.SlinghotQuantities.Should().ContainKey(slingshot.Id);
        accessoryDto.SlinghotQuantities[slingshot.Id].Should().Be(3);

        // Verify relationship in database
        var relationship = await db.AccessorySlingshots
            .FirstOrDefaultAsync(a_s => a_s.AccessoryId == accessoryDto.Id && a_s.SlingshotId == slingshot.Id);
        relationship.Should().NotBeNull();
        relationship!.Quantity.Should().Be(3);
    }

    [Fact]
    public async Task Update_UpdatesAccessory_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var accessory = new Accessory 
        { 
            Title = "Original", 
            Price = 10, 
            CategoryId = category.Id,
            Wishlist = false
        };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);
        var updateDto = new AccessoryDto(
            accessory.Id,
            "Updated",
            "http://newpic.com",
            20m,
            "http://newurl.com",
            true,
            category.Id,
            null,
            "TestCategory",
            null,
            new List<int>(),
            new List<string>(),
            new Dictionary<int, int>()
        );

        // Act
        var result = await controller.Update(accessory.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify update
        var updated = await db.Accessories.FindAsync(accessory.Id);
        updated!.Title.Should().Be("Updated");
        updated.Price.Should().Be(20m);
        updated.Wishlist.Should().BeTrue();
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new AccessoriesController(db);
        var updateDto = new AccessoryDto(1, "Test", null, 10m, null, false, 1, null, "Cat", null, new List<int>(), new List<string>(), new Dictionary<int, int>());

        // Act
        var result = await controller.Update(2, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("ID mismatch");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenAccessoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new AccessoriesController(db);
        var updateDto = new AccessoryDto(999, "Test", null, 10m, null, false, 1, null, "Cat", null, new List<int>(), new List<string>(), new Dictionary<int, int>());

        // Act
        var result = await controller.Update(999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_UpdatesSlingshotRelationships()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        var slingshot1 = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        var slingshot2 = new Slingshot { Year = 2021, Model = "Model X", Color = "Blue", UserId = "user1" };
        
        db.Categories.Add(category);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        
        db.Slingshots.AddRange(slingshot1, slingshot2);
        await db.SaveChangesAsync();

        var accessory = new Accessory { Title = "Test", Price = 10, CategoryId = category.Id, Wishlist = false };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        db.AccessorySlingshots.Add(new AccessorySlingshot { AccessoryId = accessory.Id, SlingshotId = slingshot1.Id, Quantity = 1 });
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);
        var quantities = new Dictionary<int, int> { { slingshot2.Id, 2 } };
        var updateDto = new AccessoryDto(
            accessory.Id, "Test", null, 10m, null, false, category.Id, null, "TestCategory", null,
            new List<int> { slingshot2.Id }, new List<string>(), quantities
        );

        // Act
        var result = await controller.Update(accessory.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify old relationship removed and new one added
        var relationships = await db.AccessorySlingshots
            .Where(a_s => a_s.AccessoryId == accessory.Id)
            .ToListAsync();
        relationships.Should().HaveCount(1);
        relationships[0].SlingshotId.Should().Be(slingshot2.Id);
        relationships[0].Quantity.Should().Be(2);
    }

    [Fact]
    public async Task Delete_DeletesAccessory_WhenAccessoryExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var accessory = new Accessory { Title = "Test", Price = 10, CategoryId = category.Id, Wishlist = false };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.Delete(accessory.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify deletion
        var deleted = await db.Accessories.FindAsync(accessory.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenAccessoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
