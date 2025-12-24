namespace Slingcessories.Service.Models;

public class AccessorySlingshot
{
    public int AccessoryId { get; set; }
    public Accessory Accessory { get; set; } = null!;

    public int SlingshotId { get; set; }
    public Slingshot Slingshot { get; set; } = null!;
}
