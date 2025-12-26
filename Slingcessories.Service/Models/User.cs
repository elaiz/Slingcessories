namespace Slingcessories.Service.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Accessory> Accessories { get; set; } = new List<Accessory>();
    public ICollection<Slingshot> Slingshots { get; set; } = new List<Slingshot>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
