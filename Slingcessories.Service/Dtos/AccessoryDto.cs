namespace Slingcessories.Service.Dtos;

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
    List<int>? SlinghotIds,  // Make nullable for backward compatibility
    List<string>? SlinghotDescriptions,  // Make nullable for backward compatibility
    Dictionary<int, int> SlinghotQuantities  // SlingshotId -> Quantity
);
