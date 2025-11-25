namespace Slingsessory.service.Models;

public class Subcategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<Accessory> Accessories { get; set; } = new List<Accessory>();
}
