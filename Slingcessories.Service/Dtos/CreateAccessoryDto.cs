namespace Slingcessories.Service.Dtos;

public record CreateAccessoryDto(
    string Title,
    string? PictureUrl,
    decimal Price,
    string? Url,
    bool Wishlist,
    int CategoryId,
    int? SubcategoryId,
    Dictionary<int, int> SlinghotQuantities  // SlingshotId -> Quantity
);
