namespace Slingcessories.Service.Dtos;

public record AccessoryDto(
    int Id,
    string Title,
    string? PictureUrl,   // now nullable
    int Units,
    decimal Price,
    string? Url,          // now nullable
    bool Wishlist,
    int CategoryId,
    int? SubcategoryId,
    string CategoryName,
    string? SubcategoryName,
    List<int> SlinghotIds,
    List<string> SlinghotDescriptions
);
