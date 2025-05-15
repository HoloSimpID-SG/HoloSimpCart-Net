using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HoloSimpID
{
    public class Cart : IIndexer
    {
        private static uint indexer = 0;
        public uint uDex { get; private set; }

        public string cartName;
        public bool stillOpen;
        public Simp cartOwner;

        public DateTime cartDateStart = DateTime.Now;
        public DateTime cartDatePlan;
        public DateTime cartDateEnd;

        public Dictionary<Simp, List<Item>> cartItems;
        public double costShipping;
        public int totalSimps => cartItems.Count;
        public int totalItems => cartItems.Sum(x => x.Value.Count);

        public Cart(string cartName, Simp cartOwner)
        {
            uDex = indexer++;

            this.cartName = cartName;
            this.cartOwner = cartOwner;

            cartDateStart = DateTime.Now;
            stillOpen = true;
            cartItems = new();
        }
        public double getAllCost()
        {
            double total = 0.0;
            total += costShipping;
            foreach(List<Item> listItem in cartItems.Values)
            {
                foreach (Item item in listItem)
                    total += item.priceSGD;
            }
            return total;
        }

        public double getCostPerSimp(Simp simp)
        {
            double total = 0.0;
            total += costShipping / totalSimps;
            foreach(Item item in cartItems[simp])
                total += item.priceSGD;
            return total;
        }

        public void closeCart()
        {
            stillOpen = false;
            cartDateEnd = DateTime.Now;
        }

        public override string ToString()
        {
            StringBuilder strResult = new();
            strResult.AppendLine($"Cart Name: {cartName} (id: {uDex})");
            strResult.AppendLine($"- Owned by: {cartOwner.name}");
            strResult.Append($"- Status: ");
            strResult.AppendLine(stillOpen ? $"Open" : "Close");
            strResult.AppendLine($"- Opened at: {cartDateStart}");
            strResult.AppendLine($"- Item List:");
            foreach(var kvp in cartItems)
            {
                strResult.Append($"{kvp.Key.name}: ");
                foreach(Item item in kvp.Value)
                    strResult.Append($"{item.name}, ");
                strResult.Remove(strResult.Length - 2, 2);
                strResult.Append('\n');
            }
            return strResult.ToString();
        }

        public void addItem(Item item, Simp simp)
        {
            if (stillOpen)
            {
                if (cartItems.TryGetValue(simp, out List<Item> simpItems))
                    simpItems.Add(item);
                else
                {
                    simpItems = new List<Item>();
                    simpItems.Add(item);
                    cartItems.Add(simp, simpItems);
                }
            }
        }
    }
}
