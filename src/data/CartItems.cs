using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoloSimpID
{
    public class CartItems
    {
        public int cartDex { get; set; }
        public int simpDex { get; set; }
        [ForeignKey("cartDex")]
        public Cart Cart { get; set; }

        [ForeignKey("simpDex")]
        public Simp Simp { get; set; }
        [Column(TypeName = "jsonb")]
        public List<Item> Items { get; set; } = new();
        public List<int> Quantities { get; set; } = new();
    }
}