using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoloSimpID {
  public partial class Cart {
#region Struct
    [Key]
    public int uDex { get; set; }

    //-+-+-+-+-+-+-+-+
    // Cart Details
    //-+-+-+-+-+-+-+-+
    public string CartName { get; set; } = null!;
    public int OwnerDex { get; set; }
    [ForeignKey("OwnerDex")]
    public Simp Owner { get; set; } = null!;

    //-+-+-+-+-+-+-+-+
    // Dates
    //-+-+-+-+-+-+-+-+
    [Column(TypeName = "timestamptz")]
    public DateTime DateOpen { get; set; }
    [Column(TypeName = "timestamptz")]
    public DateTime DatePlan { get; set; }
    [Column(TypeName = "timestamptz")]
    public DateTime DateClose { get; set; }
    [Column(TypeName = "timestamptz")]
    public DateTime DateDelivered { get; set; }

    public enum CartStatus { Open, Closed, Delivered }

    [NotMapped]
    public CartStatus Status => DateDelivered >= DateOpen ? CartStatus.Delivered
                                : DateClose >= DateOpen   ? CartStatus.Closed
                                                          : CartStatus.Open;

    //-+-+-+-+-+-+-+-+
    // Miscellaneous
    //-+-+-+-+-+-+-+-+
    public override string ToString() {
      return $"Cart: {CartName} (id: {uDex})";
    }
    [Column(TypeName = "numeric")]
    public decimal ShippingCost { get; set; }
#endregion
    private static readonly TimeSpan DefaultPlan = TimeSpan.FromDays(7);

    private static Cart _OpenCart(Simp owner, string cartName, DateTime? datePlan = null,
                                  decimal shippingCost = 0m) {
      return new Cart { OwnerDex      = owner.uDex,
                        CartName      = cartName,
                        DateOpen      = DateTime.UtcNow,
                        DatePlan      = datePlan ?? DateTime.UtcNow + DefaultPlan,
                        DateClose     = DateTime.MinValue.ToUniversalTime(),
                        DateDelivered = DateTime.MinValue.ToUniversalTime(),
                        ShippingCost  = shippingCost };
    }
  }
}