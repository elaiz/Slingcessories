namespace Slingcessories.Service.Models;

public class Slingshot
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    // Add UserId foreign key and navigation property
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    // Many-to-many relationship with Accessories
    public ICollection<AccessorySlingshot> AccessorySlingshots { get; set; } = new List<AccessorySlingshot>();
}
