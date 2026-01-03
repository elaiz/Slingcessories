namespace Slingcessories.Service.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Navigation properties - only Slingshots belong to users
    public ICollection<Slingshot> Slingshots { get; set; } = new List<Slingshot>();
}
