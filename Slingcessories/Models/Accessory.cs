using System.Text.Json.Serialization;

namespace Slingcessories.Models;

public class Accessory
{
    public int Id { get; set; }
    public string PictureUrl { get; set; } = string.Empty;

    [JsonPropertyName("categoryName")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("subcategoryName")]
    public string Subcategory { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public int Units { get; set; }
    public decimal Price { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool Wishlist { get; set; }
}
