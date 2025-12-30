using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Slingcessories.Service;
using Slingcessories.Service.Controllers;

namespace Slingcessories.Tests.Controllers;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsWeatherForecasts()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var controller = new WeatherForecastController(loggerMock.Object);

        // Act
        var result = controller.Get();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public void Get_ReturnsForecastsWithValidData()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var controller = new WeatherForecastController(loggerMock.Object);

        // Act
        var result = controller.Get().ToList();

        // Assert
        foreach (var forecast in result)
        {
            forecast.Date.Should().BeAfter(DateOnly.FromDateTime(DateTime.Now));
            forecast.TemperatureC.Should().BeInRange(-20, 55);
            forecast.Summary.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void Get_ReturnsArrayOfForecasts()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        var controller = new WeatherForecastController(loggerMock.Object);

        // Act
        var result = controller.Get();

        // Assert
        result.Should().BeAssignableTo<IEnumerable<WeatherForecast>>();
    }
}
