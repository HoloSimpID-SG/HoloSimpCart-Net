using RuTakingTooLong.src.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoloSimpID
{
    public class Cart : IIndexer
    {
        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        private static uint indexer = 0;
        public uint uDex => UDex; private readonly uint UDex;

        private static readonly Dictionary<uint, Cart> uDexCarts = new();
        //-+-+-+-+-+-+-+-+

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
            UDex = indexer++;
            uDexCarts.Add(uDex, this);

            this.cartName = cartName;
            this.cartOwner = cartOwner;

            cartDateStart = DateTime.Now;
            stillOpen = true;
            cartItems = new();
        }
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the cart with the <see cref="uDex"/> <paramref name="cartId"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static Cart GetCart(uint cartId)
        {
            if (uDexCarts.TryGetValue(cartId, out var cart))
                return cart;
            return null;
        }
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the first <see cref="Cart"/> with <see cref="Cart.cartName"/> that matches the <paramref name="cartName"/>.
        /// <br/> - Prioritize open carts.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static Cart GetCart(string cartName)
        {
            var carts = uDexCarts.Where(x => x.Value.cartName == cartName).Select(x => x.Value);
            if (carts.IsNullOrEmpty())
                return null;
            else
            {
                var openCarts = carts.Where(x => x.stillOpen);
                if (openCarts.IsNullOrEmpty())
                    return carts.First();
                else
                    return openCarts.First();
            }
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
