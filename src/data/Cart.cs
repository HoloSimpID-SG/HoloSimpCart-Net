using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace HoloSimpID
{
    public class Cart
    {
        [Key]
        public int uDex { get; set; }
        //-+-+-+-+-+-+-+-+
        // Cart Details
        //-+-+-+-+-+-+-+-+
        public string cartName { get; set; }
        public int cartOwnerUDex { get; set; }

        //-+-+-+-+-+-+-+-+
        // DateTimes
        //-+-+-+-+-+-+-+-+
        public DateTime cartDateStart { get; set; }
        public DateTime cartDatePlan  { get; set; }
        public DateTime cartDateEnd { get; set; }
    }
}