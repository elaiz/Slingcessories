namespace Slingcessories.Service.Models;

public class Slingshot
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    // Many-to-many relationship with Accessories
    public ICollection<AccessorySlingshot> AccessorySlingshots { get; set; } = new List<AccessorySlingshot>();
}
