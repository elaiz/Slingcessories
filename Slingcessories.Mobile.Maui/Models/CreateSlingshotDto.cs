namespace Slingcessories.Mobile.Maui.Models;

public class CreateSlingshotDto
{
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
