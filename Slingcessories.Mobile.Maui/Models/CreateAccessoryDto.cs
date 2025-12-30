namespace Slingcessories.Mobile.Maui.Models;

public record CreateAccessoryDto(
    string Title,
    string? PictureUrl,
    decimal Price,
    string? Url,
    bool Wishlist,
    int CategoryId,
    int? SubcategoryId,
    Dictionary<int, int> SlinghotQuantities
);

