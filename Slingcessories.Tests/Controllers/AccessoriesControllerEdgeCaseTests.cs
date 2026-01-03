using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Slingcessories.Service.Controllers;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Controllers;

public class AccessoriesControllerEdgeCaseTests
{
    [Fact]
    public async Task Create_Accessory_WithZeroPrice()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);
        var createDto = new CreateAccessoryDto("Free Item", null, 0m, null, false, category.Id, null, new Dictionary<int, int>());

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var dto = createdResult.Value.Should().BeOfType<AccessoryDto>().Subject;
        dto.Price.Should().Be(0m);
    }

    [Fact]
    public async Task Create_Accessory_WithEmptySlingshotQuantities()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);
        var createDto = new CreateAccessoryDto("Test", null, 10m, null, false, category.Id, null, new Dictionary<int, int>());

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var dto = createdResult.Value.Should().BeOfType<AccessoryDto>().Subject;
        dto.SlinghotQuantities.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_Accessory_WithNullSlingshotQuantities()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);
        var createDto = new CreateAccessoryDto("Test", null, 10m, null, false, category.Id, null, null!);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_Accessory_WithEmptySlingshotRelationships()
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
        var updateDto = new AccessoryDto(
            accessory.Id, "Updated", null, 20m, null, false, category.Id, null, "TestCategory", null,
            new List<int>(), new List<string>(), new Dictionary<int, int>()
        );

        // Act
        var result = await controller.Update(accessory.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_Accessory_WithNullSlingshotQuantities()
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
        var updateDto = new AccessoryDto(
            accessory.Id, "Updated", null, 20m, null, false, category.Id, null, "TestCategory", null,
            null, null, null!
        );

        // Act
        var result = await controller.Update(accessory.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAll_WithWishlistNull_ReturnsAllAccessories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        db.Accessories.AddRange(
            new Accessory { Title = "Item1", Price = 10, CategoryId = category.Id, Wishlist = true },
            new Accessory { Title = "Item2", Price = 20, CategoryId = category.Id, Wishlist = false }
        );
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accessories = okResult.Value.Should().BeAssignableTo<IEnumerable<AccessoryDto>>().Subject.ToList();
        accessories.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WithAccessoryHavingNoSubcategory()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var accessory = new Accessory 
        { 
            Title = "NoSubcategory", 
            Price = 50, 
            CategoryId = category.Id,
            SubcategoryId = null,
            Wishlist = false
        };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        var controller = new AccessoriesController(db);

        // Act
        var result = await controller.GetById(accessory.Id);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value!.SubcategoryId.Should().BeNull();
        result.Value.SubcategoryName.Should().BeNull();
    }
}
