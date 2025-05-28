
using System.Collections.Generic;
using NpgsqlTypes;

namespace HoloSimpID
{
    public class Item
    {
        [PgName("item_name")]
        public string itemName;
        [PgName("item_link")]
        public string itemLink;
        [PgName("price_sgd")]
        public double priceSGD;

        public Item(string itemName, string itemLink = "", double priceSGD = 0)
        {
            this.itemName = itemName;
            this.itemLink = itemLink;
            this.priceSGD = priceSGD;
        }

        public override string ToString() => itemName.Hyperlink(itemLink);
    }
}