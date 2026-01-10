using FluentAssertions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using Slingcessories.Services;
using System.Text.Json;

namespace Slingcessories.Tests.Blazor.Services;

/// <summary>
/// Unit tests for PageStateService (Blazor)
/// Tests page state persistence, serialization, and localStorage management
/// </summary>
public class PageStateServiceTests
{
    private readonly Mock<IJSRuntime> _jsRuntimeMock;
    private readonly PageStateService _service;

    public PageStateServiceTests()
    {
        _jsRuntimeMock = new Mock<IJSRuntime>();
        _service = new PageStateService(_jsRuntimeMock.Object);
    }

    #region SaveStateAsync Tests

    [Fact]
    public async Task SaveStateAsync_ShouldSerializeAndSaveToLocalStorage()
    {
        // Arrange
        var pageKey = "test-page";
        var testData = new TestStateModel { Value = 42, Name = "Test" };
        string? capturedJson = null;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .Callback<string, object[]>((_, args) =>
            {
                capturedJson = args[1] as string;
            })
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SaveStateAsync(pageKey, testData);

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "localStorage.setItem",
                It.Is<object[]>(args =>
                    args.Length == 2 &&
                    args[0].ToString() == "pageState_test-page")),
            Times.Once);

        capturedJson.Should().NotBeNullOrEmpty();
        var deserialized = JsonSerializer.Deserialize<TestStateModel>(capturedJson!);
        deserialized!.Value.Should().Be(42);
        deserialized.Name.Should().Be("Test");
    }

    [Fact]
    public async Task SaveStateAsync_WithDifferentTypes_ShouldSerializeCorrectly()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act & Assert - String
        await _service.SaveStateAsync("string-key", "test-value");
        _jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem",
            It.Is<object[]>(args => args[0].ToString() == "pageState_string-key")), Times.Once);

        // Act & Assert - Integer
        await _service.SaveStateAsync("int-key", 123);
        _jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem",
            It.Is<object[]>(args => args[0].ToString() == "pageState_int-key")), Times.Once);

        // Act & Assert - Boolean
        await _service.SaveStateAsync("bool-key", true);
        _jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem",
            It.Is<object[]>(args => args[0].ToString() == "pageState_bool-key")), Times.Once);
    }

    [Fact]
    public async Task SaveStateAsync_WithComplexObject_ShouldSerializeCorrectly()
    {
        // Arrange
        var complexState = new ComplexStateModel
        {
            ExpandedIds = new HashSet<int> { 1, 2, 3 },
            ViewMode = "grid",
            Filters = new Dictionary<string, string> { { "category", "electronics" }, { "status", "active" } }
        };

        string? capturedJson = null;
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .Callback<string, object[]>((_, args) =>
            {
                capturedJson = args[1] as string;
            })
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SaveStateAsync("complex-key", complexState);

        // Assert
        capturedJson.Should().NotBeNullOrEmpty();
        var deserialized = JsonSerializer.Deserialize<ComplexStateModel>(capturedJson!);
        deserialized!.ExpandedIds.Should().BeEquivalentTo(new[] { 1, 2, 3 });
        deserialized.ViewMode.Should().Be("grid");
        deserialized.Filters.Should().ContainKeys("category", "status");
    }

    [Fact]
    public async Task SaveStateAsync_WhenLocalStorageThrows_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Storage full"));

        // Act
        Func<Task> act = async () => await _service.SaveStateAsync("key", "value");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveStateAsync_WithNullValue_ShouldSerializeNull()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SaveStateAsync<string>("null-key", null!);

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()),
            Times.Once);
    }

    #endregion

    #region LoadStateAsync Tests

    [Fact]
    public async Task LoadStateAsync_WhenStateExists_ShouldDeserializeAndReturn()
    {
        // Arrange
        var pageKey = "test-page";
        var expectedData = new TestStateModel { Value = 99, Name = "Loaded" };
        var json = JsonSerializer.Serialize(expectedData);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem",
                It.Is<object[]>(args => args[0].ToString() == "pageState_test-page")))
            .ReturnsAsync(json);

        // Act
        var result = await _service.LoadStateAsync(pageKey, new TestStateModel());

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(99);
        result.Name.Should().Be("Loaded");
    }

    [Fact]
    public async Task LoadStateAsync_WhenStateDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        var pageKey = "missing-page";
        var defaultValue = new TestStateModel { Value = 10, Name = "Default" };

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _service.LoadStateAsync(pageKey, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        result.Value.Should().Be(10);
        result.Name.Should().Be("Default");
    }

    [Fact]
    public async Task LoadStateAsync_WhenEmptyString_ShouldReturnDefault()
    {
        // Arrange
        var pageKey = "empty-page";
        var defaultValue = "default-value";

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync(string.Empty);

        // Act
        var result = await _service.LoadStateAsync(pageKey, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public async Task LoadStateAsync_WhenInvalidJson_ShouldReturnDefault()
    {
        // Arrange
        var pageKey = "invalid-page";
        var defaultValue = new TestStateModel { Value = 5, Name = "Default" };

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync("invalid-json-{{{");

        // Act
        var result = await _service.LoadStateAsync(pageKey, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public async Task LoadStateAsync_WithPrimitiveTypes_ShouldDeserializeCorrectly()
    {
        // Arrange - String
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem",
                It.Is<object[]>(args => args[0].ToString() == "pageState_string-key")))
            .ReturnsAsync("\"test-string\"");

        var stringResult = await _service.LoadStateAsync("string-key", "default");
        stringResult.Should().Be("test-string");

        // Arrange - Integer
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem",
                It.Is<object[]>(args => args[0].ToString() == "pageState_int-key")))
            .ReturnsAsync("42");

        var intResult = await _service.LoadStateAsync("int-key", 0);
        intResult.Should().Be(42);

        // Arrange - Boolean
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem",
                It.Is<object[]>(args => args[0].ToString() == "pageState_bool-key")))
            .ReturnsAsync("true");

        var boolResult = await _service.LoadStateAsync("bool-key", false);
        boolResult.Should().BeTrue();
    }

    [Fact]
    public async Task LoadStateAsync_WithCollections_ShouldDeserializeCorrectly()
    {
        // Arrange
        var expectedList = new List<string> { "item1", "item2", "item3" };
        var json = JsonSerializer.Serialize(expectedList);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync(json);

        // Act
        var result = await _service.LoadStateAsync("list-key", new List<string>());

        // Assert
        result.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public async Task LoadStateAsync_WithHashSet_ShouldDeserializeCorrectly()
    {
        // Arrange
        var expectedSet = new HashSet<int> { 1, 2, 3, 4, 5 };
        var json = JsonSerializer.Serialize(expectedSet);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync(json);

        // Act
        var result = await _service.LoadStateAsync("set-key", new HashSet<int>());

        // Assert
        result.Should().BeEquivalentTo(expectedSet);
    }

    [Fact]
    public async Task LoadStateAsync_WhenLocalStorageThrows_ShouldReturnDefault()
    {
        // Arrange
        var defaultValue = "default-value";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("localStorage error"));

        // Act
        var result = await _service.LoadStateAsync("key", defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public async Task LoadStateAsync_WhenDeserializationReturnsNull_ShouldReturnDefault()
    {
        // Arrange
        var defaultValue = new TestStateModel { Value = 1, Name = "Default" };
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync("null");

        // Act
        var result = await _service.LoadStateAsync("key", defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    #endregion

    #region ClearStateAsync Tests

    [Fact]
    public async Task ClearStateAsync_ShouldRemoveFromLocalStorage()
    {
        // Arrange
        var pageKey = "test-page";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.ClearStateAsync(pageKey);

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "localStorage.removeItem",
                It.Is<object[]>(args =>
                    args.Length == 1 &&
                    args[0].ToString() == "pageState_test-page")),
            Times.Once);
    }

    [Fact]
    public async Task ClearStateAsync_WhenLocalStorageThrows_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("localStorage error"));

        // Act
        Func<Task> act = async () => await _service.ClearStateAsync("key");

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region ClearAllStatesAsync Tests

    [Fact]
    public async Task ClearAllStatesAsync_ShouldRemoveAllPageStates()
    {
        // Arrange
        var allKeys = new[] { "pageState_page1", "pageState_page2", "other_key", "pageState_page3" };
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string[]>("eval", It.IsAny<object[]>()))
            .ReturnsAsync(allKeys.Where(k => k.StartsWith("pageState_")).ToArray());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.ClearAllStatesAsync();

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem",
                It.Is<object[]>(args => args[0].ToString() == "pageState_page1")),
            Times.Once);

        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem",
                It.Is<object[]>(args => args[0].ToString() == "pageState_page2")),
            Times.Once);

        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem",
                It.Is<object[]>(args => args[0].ToString() == "pageState_page3")),
            Times.Once);

        // Verify "other_key" is NOT removed
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem",
                It.Is<object[]>(args => args[0].ToString() == "other_key")),
            Times.Never);
    }

    [Fact]
    public async Task ClearAllStatesAsync_WhenNoStatesExist_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string[]>("eval", It.IsAny<object[]>()))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        Func<Task> act = async () => await _service.ClearAllStatesAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ClearAllStatesAsync_WhenEvalThrows_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string[]>("eval", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("eval error"));

        // Act
        Func<Task> act = async () => await _service.ClearAllStatesAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public async Task Scenario_SaveAndLoadState_ShouldPersistCorrectly()
    {
        // Arrange
        var pageKey = "test-page";
        var originalState = new TestStateModel { Value = 123, Name = "Original" };
        string? savedJson = null;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .Callback<string, object[]>((_, args) => savedJson = args[1] as string)
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync(() => savedJson);

        // Act
        await _service.SaveStateAsync(pageKey, originalState);
        var loadedState = await _service.LoadStateAsync(pageKey, new TestStateModel());

        // Assert
        loadedState.Value.Should().Be(originalState.Value);
        loadedState.Name.Should().Be(originalState.Name);
    }

    [Fact]
    public async Task Scenario_SaveMultiplePagesThenClearAll_ShouldRemoveOnlyPageStates()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        var allKeys = new[] { "pageState_page1", "pageState_page2", "user_setting" };
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string[]>("eval", It.IsAny<object[]>()))
            .ReturnsAsync(new[] { "pageState_page1", "pageState_page2" });

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SaveStateAsync("page1", "state1");
        await _service.SaveStateAsync("page2", "state2");
        await _service.ClearAllStatesAsync();

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem",
                It.Is<object[]>(args => args[0].ToString()!.StartsWith("pageState_"))),
            Times.Exactly(2));
    }

    [Fact]
    public async Task Scenario_SaveLoadClear_ShouldWorkInSequence()
    {
        // Arrange
        var pageKey = "test-page";
        var state = new TestStateModel { Value = 456, Name = "Test" };
        var defaultState = new TestStateModel { Value = 0, Name = "Default" };
        string? savedJson = null;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .Callback<string, object[]>((_, args) => savedJson = args[1] as string)
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync(() => savedJson);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .Callback<string, object[]>((_, args) => savedJson = null)
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act & Assert
        // 1. Save state
        await _service.SaveStateAsync(pageKey, state);
        var loaded1 = await _service.LoadStateAsync(pageKey, defaultState);
        loaded1.Value.Should().Be(456);

        // 2. Clear state
        await _service.ClearStateAsync(pageKey);
        var loaded2 = await _service.LoadStateAsync(pageKey, defaultState);
        loaded2.Value.Should().Be(0); // Should return default
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task SaveStateAsync_WithVeryLongPageKey_ShouldHandleCorrectly()
    {
        // Arrange
        var longKey = new string('x', 500);
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        Func<Task> act = async () => await _service.SaveStateAsync(longKey, "value");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveStateAsync_WithSpecialCharactersInKey_ShouldHandleCorrectly()
    {
        // Arrange
        var specialKey = "page@key#with$special%chars&more";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SaveStateAsync(specialKey, "value");

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem",
                It.Is<object[]>(args => args[0].ToString() == $"pageState_{specialKey}")),
            Times.Once);
    }

    #endregion

    #region Test Helper Classes

    private class TestStateModel
    {
        public int Value { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class ComplexStateModel
    {
        public HashSet<int> ExpandedIds { get; set; } = new();
        public string ViewMode { get; set; } = string.Empty;
        public Dictionary<string, string> Filters { get; set; } = new();
    }

    #endregion
}
