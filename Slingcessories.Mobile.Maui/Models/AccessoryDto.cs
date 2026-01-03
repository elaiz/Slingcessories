namespace Slingcessories.Mobile.Maui.Models;

public record AccessoryDto(
    int Id,
    string Title,
    string? PictureUrl,
    decimal Price,
    string? Url,
    bool Wishlist,
    int CategoryId,
    int? SubcategoryId,
    string CategoryName,
    string? SubcategoryName,
    List<int>? SlinghotIds,
    List<string>? SlinghotDescriptions,
    Dictionary<int, int>? SlinghotQuantities
);
