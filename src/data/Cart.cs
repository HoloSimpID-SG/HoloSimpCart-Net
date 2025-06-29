using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoloSimpID
{
    public partial class Cart
    {
        #region Struct
        [Key] public int uDex { get; set; }

        //-+-+-+-+-+-+-+-+
        // Cart Details
        //-+-+-+-+-+-+-+-+
        public string CartName { get; set; }
        public int OwnerDex { get; set; }
        [ForeignKey("OwnerDex")] public Simp Owner { get; set; }

        //-+-+-+-+-+-+-+-+
        // Dates
        //-+-+-+-+-+-+-+-+
        public DateTime DateOpen { get; set; }
        public DateTime DatePlan { get; set; }
        public DateTime DateClose { get; set; }
        public DateTime DateDelivered { get; set; }

        public enum CartStatus
        {
            Open,
            Closed,
            Delivered
        }

        [NotMapped]
        public CartStatus Status =>
            DateDelivered >= DateOpen
                ? CartStatus.Delivered
                : DateClose >= DateOpen
                    ? CartStatus.Closed
                    : CartStatus.Open;

        //-+-+-+-+-+-+-+-+
        // Miscellaneous
        //-+-+-+-+-+-+-+-+
        [Column(TypeName = "numeric")] public decimal ShippingCost { get; set; }
        #endregion
        private static readonly TimeSpan DefaultPlan = TimeSpan.FromDays(7);

        private static Cart _OpenCart(Simp owner, string cartName, DateTime? datePlan = null, decimal shippingCost = 0m)
        {
            return new Cart
            {
                Owner = owner,
                CartName = cartName,
                DateOpen = DateTime.Now,
                DatePlan = datePlan ?? DateTime.Now + DefaultPlan,
                DateClose = DateTime.MinValue,
                DateDelivered = DateTime.MinValue,
                ShippingCost = shippingCost
            };
        }
    }
}