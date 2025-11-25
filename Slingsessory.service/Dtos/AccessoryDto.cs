namespace Slingsessory.service.Dtos;

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
    string? SubcategoryName
);
