using RuTakingTooLong.src.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoloSimpID
{
    public class Cart
    {
        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        #region Indexer
        private static uint indexer = 0;
        public uint uDex => UDex; private readonly uint UDex;

        private static readonly Dictionary<uint, Cart> uDexCarts = new();
        //-+-+-+-+-+-+-+-+
        #endregion

        //-+-+-+-+-+-+-+-+
        // Cart Details
        //-+-+-+-+-+-+-+-+
        private readonly string cartName;
        private bool stillOpen;
        private Simp cartOwner;

        //-+-+-+-+-+-+-+-+
        // DateTimes
        //-+-+-+-+-+-+-+-+
        public DateTime cartDateStart => CartDateStart; private readonly DateTime CartDateStart;
        public DateTime cartDatePlan => CartDatePlan; private readonly DateTime CartDatePlan;
        public DateTime cartDateEnd => CartDateEnd; private DateTime CartDateEnd;

        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        private Dictionary<Simp, Dictionary<Item, uint>> cartItems;
        public double costShipping => CostShipping; private double CostShipping;

        public Cart(string cartName, Simp cartOwner, DateTime? cartDatePlan = null)
        {
            //-+-+-+-+-+-+-+-+
            // Indexer
            //-+-+-+-+-+-+-+-+
            #region Indexer
            UDex = indexer++;
            uDexCarts.Add(uDex, this);
            //-+-+-+-+-+-+-+-+
            #endregion

            this.cartName = cartName;
            this.cartOwner = cartOwner;

            CartDateStart = DateTime.Now;
            CartDatePlan = cartDatePlan ?? DateTime.Now.AddDays(Consts.defaultCartPlan);
            stillOpen = true;
            cartItems = new();
        }
        public override string ToString() => $"{cartName}";

        //-+-+-+-+-+-+-+-+-+
        // Instance Getter
        //-+-+-+-+-+-+-+-+-+
        #region Instance Getter
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the cart with the <see cref="uDex"/> <paramref name="cartId"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static Cart GetCart(int cartId)
        {
            if (cartId < 0) return GetLastCart();

            if (uDexCarts.TryGetValue((uint)cartId, out var cart))
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
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Fills a <see cref="IList{T}"/> of <see cref="Cart"/> that fullfils the <paramref name="predicate"/>.
        /// <br/> - <paramref name="predicate"/> takes the <see langword="uint"/> for the index to access the <see cref="IDictionary{TKey, TValue}"/>.
        /// <br/> - The <see cref="IDictionary{TKey, TValue}"/> will take <see cref="uDexCarts"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static void GetAllCarts(IList<Cart> carts, Func<uint, IDictionary<uint, Cart>, bool> predicate = null)
        {
            predicate ??= (i, l) => l[i] != null;
            int len = uDexCarts.Count;
            for (uint i = 0; i < len; i++)
                if (!predicate(i, uDexCarts)) carts.Add(uDexCarts[i]);
        }
        /// <summary>
        /// <inheritdoc cref="GetAllCarts"/>
        /// </summary>
        public static List<Cart> GetAllCarts(Func<uint, IDictionary<uint, Cart>, bool> predicate = null)
        {
            List<Cart> carts = new();
            GetAllCarts(carts, predicate);
            return carts;
        }
        public static Cart GetLastCart()
        {
            if (uDexCarts.Count == 0)
                return null;
            return uDexCarts.Last().Value;
        }
        //-+-+-+-+-+-+-+-+-+
        #endregion

        //-+-+-+-+-+-+-+-+-+
        // Cart Actions
        //-+-+-+-+-+-+-+-+-+
        public void setShippingCost(double shippingCost) => CostShipping = shippingCost;
        public int totalSimps => cartItems.Count;
        public long totalItems => cartItems.Sum(x => x.Value.Sum(x => x.Value));
        public void closeCart()
        {
            // Update Status
            stillOpen = false;
            CartDateEnd = DateTime.Now;

            // Distribute all costs
            double shippingCost = costShipping / totalSimps;
            foreach (var kvp in cartItems)
            {
                Simp simp = kvp.Key;
                foreach (var itemQuantityPair in kvp.Value)
                    simp.addItemToHistory(itemQuantityPair);
                simp.addMiscSpending(shippingCost);
            }
        }
        public string getDetails()
        {
            StringBuilder strResult = new();
            strResult.AppendLine($"# {cartName} (id: {uDex})");
            strResult.AppendLine($"- Owned by: {cartOwner.name}");
            strResult.Append($"- Status: ");
            strResult.AppendLine(stillOpen ? $"Open" : "Closed");
            strResult.AppendLine($"- Opened at: {cartDateStart}");
            strResult.AppendLine($"- Item List:");
            foreach (var kvp in cartItems)
            {
                Simp simp = kvp.Key;
                strResult.AppendLine($" - {simp}:");
                foreach (var itemQuantityPair in kvp.Value)
                    strResult.AppendLine($"  - {itemQuantityPair.Key.name} ({itemQuantityPair.Key.priceSGD:C2}) {Consts.cMultiply}{itemQuantityPair.Value}");
            }
            return strResult.ToString();
        }
        public bool addItem(Simp simp, Item item, uint quantity = 1)
        {
            if (stillOpen)
            {
                Dictionary<Item, uint> simpItems;
                if (!cartItems.TryGetValue(simp, out simpItems))
                {
                    simpItems = new();
                    cartItems.Add(simp, simpItems);
                }
                simpItems.AddFrequency(item, quantity);
                return true;
            }
            return false;
        }
    }
}