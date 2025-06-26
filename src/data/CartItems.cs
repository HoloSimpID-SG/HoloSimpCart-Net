using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CartItems
{
    [Key]
    public int cartUDex { get; set; }
    [Key]
    public int simpUDex { get; set; }
    [Column(TypeName = "jsonb[]")]
    public List<string> itemJson { get; set; } = new();
    [Column(TypeName = "int[]")]
    public List<int> quantity { get; set; } = new();
}

public class Item
{
    public string itemName;
    public string itemLink;
    public double priceSGD;

    public Item(string itemName, string itemLink = "", double priceSGD = 0)
    {
        this.itemName = itemName;
        this.itemLink = itemLink;
        this.priceSGD = priceSGD;
    }

    public override string ToString() => itemName.Hyperlink(itemLink);
}