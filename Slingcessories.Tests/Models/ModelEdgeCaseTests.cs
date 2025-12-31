using FluentAssertions;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Models;

public class ModelEdgeCaseTests
{
    [Fact]
    public void Accessory_NullPictureUrl_IsAllowed()
    {
        // Arrange & Act
        var accessory = new Accessory
        {
            Title = "Test",
            PictureUrl = null,
            Price = 10m,
            CategoryId = 1,
            Wishlist = false
        };

        // Assert
        accessory.PictureUrl.Should().BeNull();
    }

    [Fact]
    public void Accessory_NullUrl_IsAllowed()
    {
        // Arrange & Act
        var accessory = new Accessory
        {
            Title = "Test",
            Url = null,
            Price = 10m,
            CategoryId = 1,
            Wishlist = false
        };

        // Assert
        accessory.Url.Should().BeNull();
    }

    [Fact]
    public void Accessory_NullSubcategoryId_IsAllowed()
    {
        // Arrange & Act
        var accessory = new Accessory
        {
            Title = "Test",
            Price = 10m,
            CategoryId = 1,
            SubcategoryId = null,
            Wishlist = false
        };

        // Assert
        accessory.SubcategoryId.Should().BeNull();
        accessory.Subcategory.Should().BeNull();
    }

    [Fact]
    public void Accessory_ZeroPrice_IsAllowed()
    {
        // Arrange & Act
        var accessory = new Accessory
        {
            Title = "Free Item",
            Price = 0m,
            CategoryId = 1,
            Wishlist = false
        };

        // Assert
        accessory.Price.Should().Be(0m);
    }

    [Fact]
    public void Accessory_HighPrecisionPrice_CanBeSet()
    {
        // Arrange & Act
        var accessory = new Accessory
        {
            Title = "Expensive",
            Price = 12345.67m,
            CategoryId = 1,
            Wishlist = false
        };

        // Assert
        accessory.Price.Should().Be(12345.67m);
    }

    [Fact]
    public void Accessory_WishlistTrue_Works()
    {
        // Arrange & Act
        var accessory = new Accessory
        {
            Title = "Wishlist Item",
            Price = 100m,
            CategoryId = 1,
            Wishlist = true
        };

        // Assert
        accessory.Wishlist.Should().BeTrue();
    }

    [Fact]
    public void Accessory_WishlistFalse_Works()
    {
        // Arrange & Act
        var accessory = new Accessory
        {
            Title = "Regular Item",
            Price = 100m,
            CategoryId = 1,
            Wishlist = false
        };

        // Assert
        accessory.Wishlist.Should().BeFalse();
    }

    [Fact]
    public void Subcategory_NullCategory_IsAllowed()
    {
        // Arrange & Act
        var subcategory = new Subcategory
        {
            Name = "Test",
            CategoryId = 1,
            Category = null
        };

        // Assert
        subcategory.Category.Should().BeNull();
    }

    [Fact]
    public void Slingshot_NullUser_IsAllowed()
    {
        // Arrange & Act
        var slingshot = new Slingshot
        {
            Year = 2020,
            Model = "Model S",
            Color = "Red",
            UserId = "user1",
            User = null
        };

        // Assert
        slingshot.User.Should().BeNull();
    }

    [Fact]
    public void AccessorySlingshot_ZeroQuantity_IsAllowed()
    {
        // Arrange & Act
        var relationship = new AccessorySlingshot
        {
            AccessoryId = 1,
            SlingshotId = 1,
            Quantity = 0
        };

        // Assert
        relationship.Quantity.Should().Be(0);
    }

    [Fact]
    public void AccessorySlingshot_HighQuantity_IsAllowed()
    {
        // Arrange & Act
        var relationship = new AccessorySlingshot
        {
            AccessoryId = 1,
            SlingshotId = 1,
            Quantity = 1000
        };

        // Assert
        relationship.Quantity.Should().Be(1000);
    }

    [Fact]
    public void Slingshot_VeryOldYear_IsAllowed()
    {
        // Arrange & Act
        var slingshot = new Slingshot
        {
            Year = 1900,
            Model = "Classic",
            Color = "Brown",
            UserId = "user1"
        };

        // Assert
        slingshot.Year.Should().Be(1900);
    }

    [Fact]
    public void Slingshot_FutureYear_IsAllowed()
    {
        // Arrange & Act
        var slingshot = new Slingshot
        {
            Year = 2050,
            Model = "Future Model",
            Color = "Silver",
            UserId = "user1"
        };

        // Assert
        slingshot.Year.Should().Be(2050);
    }

    [Fact]
    public void User_EmptyStrings_AreAllowed()
    {
        // Arrange & Act
        var user = new User
        {
            Id = "test",
            FirstName = "",
            LastName = "",
            Email = ""
        };

        // Assert
        user.FirstName.Should().BeEmpty();
        user.LastName.Should().BeEmpty();
        user.Email.Should().BeEmpty();
    }

    [Fact]
    public void Category_EmptySubcategoriesCollection_IsInitialized()
    {
        // Arrange & Act
        var category = new Category { Name = "Test" };

        // Assert
        category.Subcategories.Should().NotBeNull();
        category.Subcategories.Should().BeEmpty();
    }

    [Fact]
    public void Subcategory_EmptyAccessoriesCollection_IsInitialized()
    {
        // Arrange & Act
        var subcategory = new Subcategory { Name = "Test", CategoryId = 1 };

        // Assert
        subcategory.Accessories.Should().NotBeNull();
        subcategory.Accessories.Should().BeEmpty();
    }

    [Fact]
    public void Accessory_EmptyAccessorySlingshotsCollection_IsInitialized()
    {
        // Arrange & Act
        var accessory = new Accessory { Title = "Test", Price = 10, CategoryId = 1, Wishlist = false };

        // Assert
        accessory.AccessorySlingshots.Should().NotBeNull();
        accessory.AccessorySlingshots.Should().BeEmpty();
    }

    [Fact]
    public void Slingshot_EmptyAccessorySlingshotsCollection_IsInitialized()
    {
        // Arrange & Act
        var slingshot = new Slingshot { Year = 2020, Model = "Test", Color = "Red", UserId = "user1" };

        // Assert
        slingshot.AccessorySlingshots.Should().NotBeNull();
        slingshot.AccessorySlingshots.Should().BeEmpty();
    }

    [Fact]
    public void User_EmptySlingshotsCollection_IsInitialized()
    {
        // Arrange & Act
        var user = new User { Id = "test", FirstName = "John", LastName = "Doe", Email = "john@test.com" };

        // Assert
        user.Slingshots.Should().NotBeNull();
        user.Slingshots.Should().BeEmpty();
    }

    [Fact]
    public void AccessorySlingshot_NullAccessory_IsAllowed()
    {
        // Arrange & Act
        var relationship = new AccessorySlingshot
        {
            AccessoryId = 1,
            SlingshotId = 1,
            Quantity = 5,
            Accessory = null!
        };

        // Assert
        relationship.Accessory.Should().BeNull();
    }

    [Fact]
    public void AccessorySlingshot_NullSlingshot_IsAllowed()
    {
        // Arrange & Act
        var relationship = new AccessorySlingshot
        {
            AccessoryId = 1,
            SlingshotId = 1,
            Quantity = 5,
            Slingshot = null!
        };

        // Assert
        relationship.Slingshot.Should().BeNull();
    }
}
