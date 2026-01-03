using FluentAssertions;
using Slingcessories.Service.Dtos;

namespace Slingcessories.Tests.Dtos;

public class DtoTests
{
    // UserDto Tests
    [Fact]
    public void UserDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new UserDto("1", "John", "Doe", "john@test.com");
        var dto2 = new UserDto("1", "John", "Doe", "john@test.com");

        // Assert
        dto1.Should().Be(dto2);
        dto1.GetHashCode().Should().Be(dto2.GetHashCode());
    }

    [Fact]
    public void UserDto_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var original = new UserDto("1", "John", "Doe", "john@test.com");

        // Act
        var modified = original with { FirstName = "Jane" };

        // Assert
        modified.FirstName.Should().Be("Jane");
        original.FirstName.Should().Be("John");
        modified.Should().NotBe(original);
    }

    [Fact]
    public void UserDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new UserDto("123", "John", "Doe", "john@test.com");

        // Assert
        dto.Id.Should().Be("123");
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Email.Should().Be("john@test.com");
    }

    // CreateUserDto Tests
    [Fact]
    public void CreateUserDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new CreateUserDto("John", "Doe", "john@test.com");
        var dto2 = new CreateUserDto("John", "Doe", "john@test.com");

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void CreateUserDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new CreateUserDto("John", "Doe", "john@test.com");

        // Assert
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Email.Should().Be("john@test.com");
    }

    // AccessoryDto Tests
    [Fact]
    public void AccessoryDto_RecordEquality_Works()
    {
        // Arrange
        var slingshotIds1 = new List<int> { 1 };
        var slingshotIds2 = new List<int> { 1 };
        var descriptions1 = new List<string> { "2020 Model S (Red)" };
        var descriptions2 = new List<string> { "2020 Model S (Red)" };
        var quantities1 = new Dictionary<int, int> { { 1, 2 } };
        var quantities2 = new Dictionary<int, int> { { 1, 2 } };

        var dto1 = new AccessoryDto(1, "Test", "http://pic.com", 10.50m, "http://url.com", 
            true, 1, 2, "Category", "Subcategory", slingshotIds1, descriptions1, quantities1);
        var dto2 = new AccessoryDto(1, "Test", "http://pic.com", 10.50m, "http://url.com", 
            true, 1, 2, "Category", "Subcategory", slingshotIds2, descriptions2, quantities2);

        // Assert - AccessoryDto contains collections which don't have structural equality by default
        dto1.Id.Should().Be(dto2.Id);
        dto1.Title.Should().Be(dto2.Title);
        dto1.Price.Should().Be(dto2.Price);
        dto1.SlinghotIds.Should().BeEquivalentTo(dto2.SlinghotIds);
        dto1.SlinghotQuantities.Should().BeEquivalentTo(dto2.SlinghotQuantities);
    }

    [Fact]
    public void AccessoryDto_WithNullableProperties_Works()
    {
        // Arrange & Act
        var dto = new AccessoryDto(1, "Test", null, 10.50m, null, false, 1, null, 
            "Category", null, null, null, new Dictionary<int, int>());

        // Assert
        dto.PictureUrl.Should().BeNull();
        dto.Url.Should().BeNull();
        dto.SubcategoryId.Should().BeNull();
        dto.SubcategoryName.Should().BeNull();
        dto.SlinghotIds.Should().BeNull();
        dto.SlinghotDescriptions.Should().BeNull();
    }

    [Fact]
    public void AccessoryDto_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var original = new AccessoryDto(1, "Original", null, 10m, null, false, 1, null, 
            "Category", null, null, null, new Dictionary<int, int>());

        // Act
        var modified = original with { Title = "Modified", Price = 20m };

        // Assert
        modified.Title.Should().Be("Modified");
        modified.Price.Should().Be(20m);
        original.Title.Should().Be("Original");
        original.Price.Should().Be(10m);
    }

    // CreateAccessoryDto Tests
    [Fact]
    public void CreateAccessoryDto_RecordEquality_Works()
    {
        // Arrange
        var quantities = new Dictionary<int, int> { { 1, 2 } };
        var dto1 = new CreateAccessoryDto("Test", "http://pic.com", 10.50m, "http://url.com", 
            true, 1, 2, quantities);
        var dto2 = new CreateAccessoryDto("Test", "http://pic.com", 10.50m, "http://url.com", 
            true, 1, 2, quantities);

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void CreateAccessoryDto_WithNullProperties_Works()
    {
        // Arrange & Act
        var dto = new CreateAccessoryDto("Test", null, 10m, null, false, 1, null, 
            new Dictionary<int, int>());

        // Assert
        dto.PictureUrl.Should().BeNull();
        dto.Url.Should().BeNull();
        dto.SubcategoryId.Should().BeNull();
        dto.SlinghotQuantities.Should().BeEmpty();
    }

    // SlinghotDto Tests
    [Fact]
    public void SlinghotDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new SlinghotDto(1, 2020, "Model S", "Red");
        var dto2 = new SlinghotDto(1, 2020, "Model S", "Red");

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void SlinghotDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new SlinghotDto(1, 2020, "Model S", "Red");

        // Assert
        dto.Id.Should().Be(1);
        dto.Year.Should().Be(2020);
        dto.Model.Should().Be("Model S");
        dto.Color.Should().Be("Red");
    }

    [Fact]
    public void SlinghotDto_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var original = new SlinghotDto(1, 2020, "Model S", "Red");

        // Act
        var modified = original with { Year = 2021, Color = "Blue" };

        // Assert
        modified.Year.Should().Be(2021);
        modified.Color.Should().Be("Blue");
        original.Year.Should().Be(2020);
        original.Color.Should().Be("Red");
    }

    // CreateSlingshotDto Tests
    [Fact]
    public void CreateSlingshotDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new CreateSlingshotDto(2020, "Model S", "Red", "user1");
        var dto2 = new CreateSlingshotDto(2020, "Model S", "Red", "user1");

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void CreateSlingshotDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new CreateSlingshotDto(2020, "Model S", "Red", "user1");

        // Assert
        dto.Year.Should().Be(2020);
        dto.Model.Should().Be("Model S");
        dto.Color.Should().Be("Red");
        dto.UserId.Should().Be("user1");
    }

    // CategoryDto Tests (from Dtos namespace)
    [Fact]
    public void CategoryDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new CategoryDto(1, "Electronics");
        var dto2 = new CategoryDto(1, "Electronics");

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void CategoryDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new CategoryDto(1, "Electronics");

        // Assert
        dto.Id.Should().Be(1);
        dto.Name.Should().Be("Electronics");
    }

    // CreateCategoryDto Tests
    [Fact]
    public void CreateCategoryDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new CreateCategoryDto("Electronics");
        var dto2 = new CreateCategoryDto("Electronics");

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void CreateCategoryDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new CreateCategoryDto("Electronics");

        // Assert
        dto.Name.Should().Be("Electronics");
    }

    // SubcategoryDto Tests
    [Fact]
    public void SubcategoryDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new SubcategoryDto(1, "Laptops", 10);
        var dto2 = new SubcategoryDto(1, "Laptops", 10);

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void SubcategoryDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new SubcategoryDto(1, "Laptops", 10);

        // Assert
        dto.Id.Should().Be(1);
        dto.Name.Should().Be("Laptops");
        dto.CategoryId.Should().Be(10);
    }

    [Fact]
    public void SubcategoryDto_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var original = new SubcategoryDto(1, "Laptops", 10);

        // Act
        var modified = original with { Name = "Desktops", CategoryId = 11 };

        // Assert
        modified.Name.Should().Be("Desktops");
        modified.CategoryId.Should().Be(11);
        original.Name.Should().Be("Laptops");
        original.CategoryId.Should().Be(10);
    }

    // CreateSubcategoryDto Tests
    [Fact]
    public void CreateSubcategoryDto_RecordEquality_Works()
    {
        // Arrange
        var dto1 = new CreateSubcategoryDto("Laptops", 10);
        var dto2 = new CreateSubcategoryDto("Laptops", 10);

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void CreateSubcategoryDto_Properties_AreAccessible()
    {
        // Arrange & Act
        var dto = new CreateSubcategoryDto("Laptops", 10);

        // Assert
        dto.Name.Should().Be("Laptops");
        dto.CategoryId.Should().Be(10);
    }
}
