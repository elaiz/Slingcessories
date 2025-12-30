using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Controllers;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Controllers;

public class SlingshotsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoSlingshots()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var slingshots = okResult.Value.Should().BeAssignableTo<IEnumerable<SlinghotDto>>().Subject;
        slingshots.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnsAllSlingshots_OrderedByYearThenModel()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Slingshots.AddRange(
            new Slingshot { Year = 2022, Model = "Model A", Color = "Red", UserId = "user1" },
            new Slingshot { Year = 2020, Model = "Model B", Color = "Blue", UserId = "user1" },
            new Slingshot { Year = 2020, Model = "Model A", Color = "Green", UserId = "user1" }
        );
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var slingshots = okResult.Value.Should().BeAssignableTo<IEnumerable<SlinghotDto>>().Subject.ToList();
        slingshots.Should().HaveCount(3);
        slingshots[0].Year.Should().Be(2020);
        slingshots[0].Model.Should().Be("Model A");
        slingshots[1].Year.Should().Be(2020);
        slingshots[1].Model.Should().Be("Model B");
        slingshots[2].Year.Should().Be(2022);
    }

    [Fact]
    public async Task GetAll_FiltersById_WhenUserIdProvided()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user1 = new User { Id = "user1", FirstName = "User", LastName = "One", Email = "user1@test.com" };
        var user2 = new User { Id = "user2", FirstName = "User", LastName = "Two", Email = "user2@test.com" };
        db.Users.AddRange(user1, user2);
        await db.SaveChangesAsync();

        db.Slingshots.AddRange(
            new Slingshot { Year = 2020, Model = "Model A", Color = "Red", UserId = "user1" },
            new Slingshot { Year = 2021, Model = "Model B", Color = "Blue", UserId = "user2" },
            new Slingshot { Year = 2022, Model = "Model C", Color = "Green", UserId = "user1" }
        );
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.GetAll("user1");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var slingshots = okResult.Value.Should().BeAssignableTo<IEnumerable<SlinghotDto>>().Subject.ToList();
        slingshots.Should().HaveCount(2);
        slingshots.Should().OnlyContain(s => s.Model == "Model A" || s.Model == "Model C");
    }

    [Fact]
    public async Task GetAll_DoesNotFilter_WhenUserIdIsEmpty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Slingshots.Add(new Slingshot { Year = 2020, Model = "Model A", Color = "Red", UserId = "user1" });
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.GetAll(string.Empty);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var slingshots = okResult.Value.Should().BeAssignableTo<IEnumerable<SlinghotDto>>().Subject.ToList();
        slingshots.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_ReturnsSlingshot_WhenSlingshotExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.GetById(slingshot.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<SlinghotDto>().Subject;
        dto.Id.Should().Be(slingshot.Id);
        dto.Year.Should().Be(2020);
        dto.Model.Should().Be("Model S");
        dto.Color.Should().Be("Red");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenSlingshotDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_CreatesSlingshot_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);
        var createDto = new CreateSlingshotDto(2021, "Model X", "Blue", "user1");

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var dto = createdResult.Value.Should().BeOfType<SlinghotDto>().Subject;
        dto.Year.Should().Be(2021);
        dto.Model.Should().Be("Model X");
        dto.Color.Should().Be("Blue");

        // Verify saved to database
        var saved = await db.Slingshots.FindAsync(dto.Id);
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be("user1");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelIsEmpty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);
        var createDto = new CreateSlingshotDto(2021, "", "Blue", "user1");

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Model is required.");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelIsWhitespace()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);
        var createDto = new CreateSlingshotDto(2021, "   ", "Blue", "user1");

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Model is required.");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenColorIsEmpty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);
        var createDto = new CreateSlingshotDto(2021, "Model X", "", "user1");

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Color is required.");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenColorIsWhitespace()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);
        var createDto = new CreateSlingshotDto(2021, "Model X", "   ", "user1");

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Color is required.");
    }

    [Fact]
    public async Task Update_UpdatesSlingshot_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);
        var updateDto = new SlinghotDto(slingshot.Id, 2021, "Model X", "Blue");

        // Act
        var result = await controller.Update(slingshot.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify update
        var updated = await db.Slingshots.FindAsync(slingshot.Id);
        updated!.Year.Should().Be(2021);
        updated.Model.Should().Be("Model X");
        updated.Color.Should().Be("Blue");
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);
        var updateDto = new SlinghotDto(1, 2021, "Model X", "Blue");

        // Act
        var result = await controller.Update(2, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("ID mismatch.");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenSlingshotDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);
        var updateDto = new SlinghotDto(999, 2021, "Model X", "Blue");

        // Act
        var result = await controller.Update(999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenModelIsEmpty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);
        var updateDto = new SlinghotDto(slingshot.Id, 2021, "", "Blue");

        // Act
        var result = await controller.Update(slingshot.Id, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Model is required.");
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenColorIsEmpty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);
        var updateDto = new SlinghotDto(slingshot.Id, 2021, "Model X", "");

        // Act
        var result = await controller.Update(slingshot.Id, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Color is required.");
    }

    [Fact]
    public async Task Delete_DeletesSlingshot_WhenSlingshotExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.Delete(slingshot.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify deletion
        var deleted = await db.Slingshots.FindAsync(slingshot.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenSlingshotDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenSlingshotHasAccessories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "user1", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        var category = new Category { Name = "TestCategory" };
        db.Users.Add(user);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        db.Slingshots.Add(slingshot);
        await db.SaveChangesAsync();

        var accessory = new Accessory { Title = "TestAccessory", Price = 10, CategoryId = category.Id, Wishlist = false };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        db.AccessorySlingshots.Add(new AccessorySlingshot 
        { 
            AccessoryId = accessory.Id, 
            SlingshotId = slingshot.Id,
            Quantity = 1
        });
        await db.SaveChangesAsync();

        var controller = new SlingshotsController(db);

        // Act
        var result = await controller.Delete(slingshot.Id);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Cannot delete slingshot with existing accessories.");
    }
}
