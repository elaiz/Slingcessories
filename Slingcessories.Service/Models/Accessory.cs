namespace Slingcessories.Service.Models;

public class Accessory
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public decimal Price { get; set; }
    public string? Url { get; set; }
    public bool Wishlist { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }

    // Many-to-many relationship with Slingshots
    public ICollection<AccessorySlingshot> AccessorySlingshots { get; set; } = new List<AccessorySlingshot>();
}
