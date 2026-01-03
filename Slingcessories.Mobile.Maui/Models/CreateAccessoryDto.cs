namespace Slingcessories.Mobile.Maui.Models;

public class CreateAccessoryDto
{
    public string Title { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public decimal Price { get; set; }
    public string? Url { get; set; }
    public bool Wishlist { get; set; }
    public int CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public Dictionary<int, int>? SlinghotQuantities { get; set; }
}
