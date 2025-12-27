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
    public decimal Price { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool Wishlist { get; set; }
    
    [JsonPropertyName("slinghotDescriptions")]
    public List<string>? SlinghotDescriptions { get; set; }
    
    [JsonPropertyName("slinghotQuantities")]
    public Dictionary<int, int>? SlinghotQuantities { get; set; }
}
