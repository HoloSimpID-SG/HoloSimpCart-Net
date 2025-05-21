
using System.Collections.Generic;

namespace HoloSimpID
{
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
}