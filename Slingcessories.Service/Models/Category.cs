namespace Slingcessories.Service.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
