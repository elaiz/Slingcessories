namespace Slingsessory.service.Dtos;

public record CreateAccessoryDto(
    string Title,
    string PictureUrl,
    int Units,
    decimal Price,
    string Url,
    bool Wishlist,
    int CategoryId,
    int? SubcategoryId
);
