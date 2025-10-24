using System.ComponentModel.DataAnnotations.Schema;

namespace HoloSimpID {
  public class CartItems {
    public int cartDex { get; set; }
    public int simpDex { get; set; }
    [ForeignKey("cartDex")]
    public Cart Cart { get; set; } = null!;

    [ForeignKey("simpDex")]
    public Simp Simp { get; set; } = null!;
    [Column(TypeName = "jsonb")]
    public List<Item> Items { get; set; }     = new();
    public List<int> Quantities { get; set; } = new();
  }
}