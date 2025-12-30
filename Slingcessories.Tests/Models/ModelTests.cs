using FluentAssertions;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Models;

public class ModelTests
{
    [Fact]
    public void User_CanBeCreatedWithDefaultValues()
    {
        // Act
        var user = new User();

        // Assert
        user.Id.Should().NotBeNullOrEmpty();
        user.FirstName.Should().BeEmpty();
        user.LastName.Should().BeEmpty();
        user.Email.Should().BeEmpty();
        user.Slingshots.Should().NotBeNull();
        user.Slingshots.Should().BeEmpty();
    }

    [Fact]
    public void User_CanSetProperties()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = "custom-id";
        user.FirstName = "John";
        user.LastName = "Doe";
        user.Email = "john@test.com";

        // Assert
        user.Id.Should().Be("custom-id");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john@test.com");
    }

    [Fact]
    public void User_GeneratesGuidId()
    {
        // Act
        var user1 = new User();
        var user2 = new User();

        // Assert
        user1.Id.Should().NotBe(user2.Id);
        Guid.TryParse(user1.Id, out _).Should().BeTrue();
        Guid.TryParse(user2.Id, out _).Should().BeTrue();
    }

    [Fact]
    public void Category_CanBeCreatedWithDefaultValues()
    {
        // Act
        var category = new Category();

        // Assert
        category.Id.Should().Be(0);
        category.Name.Should().BeEmpty();
        category.Subcategories.Should().NotBeNull();
        category.Subcategories.Should().BeEmpty();
    }

    [Fact]
    public void Category_CanSetProperties()
    {
        // Arrange
        var category = new Category();

        // Act
        category.Id = 1;
        category.Name = "TestCategory";

        // Assert
        category.Id.Should().Be(1);
        category.Name.Should().Be("TestCategory");
    }

    [Fact]
    public void Subcategory_CanBeCreatedWithDefaultValues()
    {
        // Act
        var subcategory = new Subcategory();

        // Assert
        subcategory.Id.Should().Be(0);
        subcategory.Name.Should().BeEmpty();
        subcategory.CategoryId.Should().Be(0);
        subcategory.Category.Should().BeNull();
        subcategory.Accessories.Should().NotBeNull();
        subcategory.Accessories.Should().BeEmpty();
    }

    [Fact]
    public void Subcategory_CanSetProperties()
    {
        // Arrange
        var subcategory = new Subcategory();
        var category = new Category { Id = 1, Name = "TestCategory" };

        // Act
        subcategory.Id = 10;
        subcategory.Name = "TestSubcategory";
        subcategory.CategoryId = 1;
        subcategory.Category = category;

        // Assert
        subcategory.Id.Should().Be(10);
        subcategory.Name.Should().Be("TestSubcategory");
        subcategory.CategoryId.Should().Be(1);
        subcategory.Category.Should().NotBeNull();
        subcategory.Category!.Name.Should().Be("TestCategory");
    }

    [Fact]
    public void Accessory_CanBeCreatedWithDefaultValues()
    {
        // Act
        var accessory = new Accessory();

        // Assert
        accessory.Id.Should().Be(0);
        accessory.Title.Should().BeEmpty();
        accessory.PictureUrl.Should().BeNull();
        accessory.Price.Should().Be(0);
        accessory.Url.Should().BeNull();
        accessory.Wishlist.Should().BeFalse();
        accessory.CategoryId.Should().Be(0);
        accessory.Category.Should().BeNull();
        accessory.SubcategoryId.Should().BeNull();
        accessory.Subcategory.Should().BeNull();
        accessory.AccessorySlingshots.Should().NotBeNull();
        accessory.AccessorySlingshots.Should().BeEmpty();
    }

    [Fact]
    public void Accessory_CanSetProperties()
    {
        // Arrange
        var accessory = new Accessory();
        var category = new Category { Id = 1, Name = "TestCategory" };
        var subcategory = new Subcategory { Id = 10, Name = "TestSubcategory", CategoryId = 1 };

        // Act
        accessory.Id = 100;
        accessory.Title = "TestAccessory";
        accessory.PictureUrl = "http://test.com/pic.jpg";
        accessory.Price = 99.99m;
        accessory.Url = "http://test.com";
        accessory.Wishlist = true;
        accessory.CategoryId = 1;
        accessory.Category = category;
        accessory.SubcategoryId = 10;
        accessory.Subcategory = subcategory;

        // Assert
        accessory.Id.Should().Be(100);
        accessory.Title.Should().Be("TestAccessory");
        accessory.PictureUrl.Should().Be("http://test.com/pic.jpg");
        accessory.Price.Should().Be(99.99m);
        accessory.Url.Should().Be("http://test.com");
        accessory.Wishlist.Should().BeTrue();
        accessory.CategoryId.Should().Be(1);
        accessory.Category.Should().NotBeNull();
        accessory.SubcategoryId.Should().Be(10);
        accessory.Subcategory.Should().NotBeNull();
    }

    [Fact]
    public void Slingshot_CanBeCreatedWithDefaultValues()
    {
        // Act
        var slingshot = new Slingshot();

        // Assert
        slingshot.Id.Should().Be(0);
        slingshot.Year.Should().Be(0);
        slingshot.Model.Should().BeEmpty();
        slingshot.Color.Should().BeEmpty();
        slingshot.UserId.Should().BeEmpty();
        slingshot.User.Should().BeNull();
        slingshot.AccessorySlingshots.Should().NotBeNull();
        slingshot.AccessorySlingshots.Should().BeEmpty();
    }

    [Fact]
    public void Slingshot_CanSetProperties()
    {
        // Arrange
        var slingshot = new Slingshot();
        var user = new User { Id = "user1", FirstName = "John", LastName = "Doe", Email = "john@test.com" };

        // Act
        slingshot.Id = 1;
        slingshot.Year = 2020;
        slingshot.Model = "Model S";
        slingshot.Color = "Red";
        slingshot.UserId = "user1";
        slingshot.User = user;

        // Assert
        slingshot.Id.Should().Be(1);
        slingshot.Year.Should().Be(2020);
        slingshot.Model.Should().Be("Model S");
        slingshot.Color.Should().Be("Red");
        slingshot.UserId.Should().Be("user1");
        slingshot.User.Should().NotBeNull();
        slingshot.User!.Email.Should().Be("john@test.com");
    }

    [Fact]
    public void AccessorySlingshot_CanBeCreatedWithDefaultValues()
    {
        // Act
        var accessorySlingshot = new AccessorySlingshot();

        // Assert
        accessorySlingshot.AccessoryId.Should().Be(0);
        accessorySlingshot.Accessory.Should().BeNull();
        accessorySlingshot.SlingshotId.Should().Be(0);
        accessorySlingshot.Slingshot.Should().BeNull();
        accessorySlingshot.Quantity.Should().Be(0);
    }

    [Fact]
    public void AccessorySlingshot_CanSetProperties()
    {
        // Arrange
        var accessorySlingshot = new AccessorySlingshot();
        var accessory = new Accessory { Id = 100, Title = "TestAccessory", Price = 10, Wishlist = false };
        var slingshot = new Slingshot { Id = 1, Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };

        // Act
        accessorySlingshot.AccessoryId = 100;
        accessorySlingshot.Accessory = accessory;
        accessorySlingshot.SlingshotId = 1;
        accessorySlingshot.Slingshot = slingshot;
        accessorySlingshot.Quantity = 5;

        // Assert
        accessorySlingshot.AccessoryId.Should().Be(100);
        accessorySlingshot.Accessory.Should().NotBeNull();
        accessorySlingshot.Accessory!.Title.Should().Be("TestAccessory");
        accessorySlingshot.SlingshotId.Should().Be(1);
        accessorySlingshot.Slingshot.Should().NotBeNull();
        accessorySlingshot.Slingshot!.Model.Should().Be("Model S");
        accessorySlingshot.Quantity.Should().Be(5);
    }

    [Fact]
    public void User_SlingshotsCollection_CanAddAndRemove()
    {
        // Arrange
        var user = new User { Id = "user1", FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        var slingshot1 = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        var slingshot2 = new Slingshot { Year = 2021, Model = "Model X", Color = "Blue", UserId = "user1" };

        // Act
        user.Slingshots.Add(slingshot1);
        user.Slingshots.Add(slingshot2);

        // Assert
        user.Slingshots.Should().HaveCount(2);
        user.Slingshots.Should().Contain(slingshot1);
        user.Slingshots.Should().Contain(slingshot2);

        // Act - Remove
        user.Slingshots.Remove(slingshot1);

        // Assert
        user.Slingshots.Should().HaveCount(1);
        user.Slingshots.Should().NotContain(slingshot1);
    }

    [Fact]
    public void Category_SubcategoriesCollection_CanAddAndRemove()
    {
        // Arrange
        var category = new Category { Name = "TestCategory" };
        var subcategory1 = new Subcategory { Name = "Sub1", CategoryId = 1 };
        var subcategory2 = new Subcategory { Name = "Sub2", CategoryId = 1 };

        // Act
        category.Subcategories.Add(subcategory1);
        category.Subcategories.Add(subcategory2);

        // Assert
        category.Subcategories.Should().HaveCount(2);
        category.Subcategories.Should().Contain(subcategory1);
        category.Subcategories.Should().Contain(subcategory2);

        // Act - Remove
        category.Subcategories.Remove(subcategory1);

        // Assert
        category.Subcategories.Should().HaveCount(1);
        category.Subcategories.Should().NotContain(subcategory1);
    }

    [Fact]
    public void Subcategory_AccessoriesCollection_CanAddAndRemove()
    {
        // Arrange
        var subcategory = new Subcategory { Name = "TestSubcategory", CategoryId = 1 };
        var accessory1 = new Accessory { Title = "Accessory1", Price = 10, CategoryId = 1, SubcategoryId = 1, Wishlist = false };
        var accessory2 = new Accessory { Title = "Accessory2", Price = 20, CategoryId = 1, SubcategoryId = 1, Wishlist = false };

        // Act
        subcategory.Accessories.Add(accessory1);
        subcategory.Accessories.Add(accessory2);

        // Assert
        subcategory.Accessories.Should().HaveCount(2);
        subcategory.Accessories.Should().Contain(accessory1);
        subcategory.Accessories.Should().Contain(accessory2);

        // Act - Remove
        subcategory.Accessories.Remove(accessory1);

        // Assert
        subcategory.Accessories.Should().HaveCount(1);
        subcategory.Accessories.Should().NotContain(accessory1);
    }

    [Fact]
    public void Accessory_AccessorySlingshotsCollection_CanAddAndRemove()
    {
        // Arrange
        var accessory = new Accessory { Title = "TestAccessory", Price = 10, CategoryId = 1, Wishlist = false };
        var relationship1 = new AccessorySlingshot { AccessoryId = 1, SlingshotId = 1, Quantity = 2 };
        var relationship2 = new AccessorySlingshot { AccessoryId = 1, SlingshotId = 2, Quantity = 3 };

        // Act
        accessory.AccessorySlingshots.Add(relationship1);
        accessory.AccessorySlingshots.Add(relationship2);

        // Assert
        accessory.AccessorySlingshots.Should().HaveCount(2);
        accessory.AccessorySlingshots.Should().Contain(relationship1);
        accessory.AccessorySlingshots.Should().Contain(relationship2);

        // Act - Remove
        accessory.AccessorySlingshots.Remove(relationship1);

        // Assert
        accessory.AccessorySlingshots.Should().HaveCount(1);
        accessory.AccessorySlingshots.Should().NotContain(relationship1);
    }

    [Fact]
    public void Slingshot_AccessorySlingshotsCollection_CanAddAndRemove()
    {
        // Arrange
        var slingshot = new Slingshot { Year = 2020, Model = "Model S", Color = "Red", UserId = "user1" };
        var relationship1 = new AccessorySlingshot { AccessoryId = 1, SlingshotId = 1, Quantity = 2 };
        var relationship2 = new AccessorySlingshot { AccessoryId = 2, SlingshotId = 1, Quantity = 3 };

        // Act
        slingshot.AccessorySlingshots.Add(relationship1);
        slingshot.AccessorySlingshots.Add(relationship2);

        // Assert
        slingshot.AccessorySlingshots.Should().HaveCount(2);
        slingshot.AccessorySlingshots.Should().Contain(relationship1);
        slingshot.AccessorySlingshots.Should().Contain(relationship2);

        // Act - Remove
        slingshot.AccessorySlingshots.Remove(relationship1);

        // Assert
        slingshot.AccessorySlingshots.Should().HaveCount(1);
        slingshot.AccessorySlingshots.Should().NotContain(relationship1);
    }
}
