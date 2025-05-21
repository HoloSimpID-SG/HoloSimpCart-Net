using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
        private readonly Simp cartOwner;    
        public bool stillOpen => cartDateEnd < cartDateStart;

        //-+-+-+-+-+-+-+-+
        // DateTimes
        //-+-+-+-+-+-+-+-+
        public DateTime cartDateStart => CartDateStart; private readonly DateTime CartDateStart;
        public DateTime cartDatePlan => CartDatePlan; private readonly DateTime CartDatePlan;
        public DateTime cartDateEnd => CartDateEnd; private DateTime CartDateEnd;

        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        private readonly Dictionary<Simp, Dictionary<Item, uint>> cartItems;
        public double costShipping => CostShipping; private double CostShipping;

        public Cart(string cartName, Simp cartOwner, DateTime? cartDateStart = null, DateTime? cartDatePlan = null)
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

            CartDateStart = cartDateStart ?? DateTime.Now;
            CartDatePlan = cartDatePlan ?? DateTime.Now.AddDays(Consts.defaultCartPlan);
            CartDateEnd = CartDateStart.AddTicks(-1);
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
        #region Cart Actions
        public void setShippingCost(double shippingCost) => CostShipping = shippingCost;
        public int totalSimps => cartItems.Count;
        public long totalItems => cartItems.Sum(x => x.Value.Sum(x => x.Value));
        public void closeCart(DateTime? dateTime = null)
        {
            // Update Status
            CartDateEnd = dateTime ?? DateTime.Now;

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
            strResult.AppendLine($"- Owned by: {cartOwner.simpName}");
            strResult.Append($"- Status: ");
            strResult.AppendLine(stillOpen ? $"Open" : "Closed");
            strResult.AppendLine($"- Opened at: {cartDateStart}");
            strResult.AppendLine($"- Item List:");
            foreach (var kvp in cartItems)
            {
                Simp simp = kvp.Key;
                strResult.AppendLine($" - {simp}:");
                foreach (var itemQuantityPair in kvp.Value)
                    strResult.AppendLine($"  - {itemQuantityPair.Key.itemName} ({itemQuantityPair.Key.priceSGD:C2}) {Consts.cMultiply}{itemQuantityPair.Value}");
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
        #endregion

        //-+-+-+-+-+-+-+-+-+
        // Database
        //-+-+-+-+-+-+-+-+-+
        #region Database
        const string sqlTableName = "Carts";
        const string sqlTableNameCartItems = "CartItems";
        public static List<SqlCommand> SerializeAll()
        {
            List<SqlCommand> sqlCommands = new();
            SerializeAll(sqlCommands);
            return sqlCommands;
        }
        public static void SerializeAll(IList<SqlCommand> sqlCommands)
        {
            foreach (Cart cart in uDexCarts.Values)
                cart.Serialize(sqlCommands);
        }
        public List<SqlCommand> Serialize()
        {
            List<SqlCommand> sqlCommands = new();
            Serialize(sqlCommands);
            return sqlCommands;
        }
        public void Serialize(IList<SqlCommand> sqlCommands)
        {
            StringBuilder strCommand = new();
            //-+-+-+-+-+-+-+-+
            // Create Table if not exists
            //-+-+-+-+-+-+-+-+
            strCommand.Clear();
            strCommand.Append($"CREATE TABLE IF NOT EXISTS {sqlTableName}");
            strCommand.Append($"(");
            strCommand.Append($"cartName {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($"ownerId {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($"cartDateStart {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($"cartDatePlan {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($"cartDateEnd {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($"costShipping {MoLibrary.sqlDataType[typeof(double)]}, ");
            strCommand.Append($")");
            var cmdTable = new SqlCommand(strCommand.ToString());
            sqlCommands.Add(cmdTable);
            strCommand.Clear();
            strCommand.Append($"CREATE TABLE IF NOT EXISTS {sqlTableNameCartItems}");
            strCommand.Append($"(");
            strCommand.Append($"cartId {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($"ownerId {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($"itemName {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($"itemLink {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($"itemPrice {MoLibrary.sqlDataType[typeof(double)]}, ");
            strCommand.Append($"quantity {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($")");
            var cmdTableMerch = new SqlCommand(strCommand.ToString());
            sqlCommands.Add(cmdTableMerch);
            //-+-+-+-+-+-+-+-+

            strCommand.Clear();
            strCommand.Append($"INSERT INTO {sqlTableName}");
            strCommand.Append($"(cartName, ownerId, cartDateStart, cartDatePlan, cartDateEnd, costShipping) ");
            strCommand.Append($"VALUES ");
            strCommand.Append($"(@cartName, @ownerId, @cartDateStart, @cartDatePlan, @cartDateEnd, @costShipping) ");
            SqlCommand cmdCart = new SqlCommand(strCommand.ToString());
            cmdCart.Parameters.AddWithValue("@cartName", cartName);
            cmdCart.Parameters.AddWithValue("@ownerId", cartOwner.uDex);
            cmdCart.Parameters.AddWithValue("@cartDateStart", cartDateStart.ToSqlDate());
            cmdCart.Parameters.AddWithValue("@cartDatePlan", cartDatePlan.ToSqlDate());
            cmdCart.Parameters.AddWithValue("@cartDateEnd", cartDateEnd.ToSqlDate());
            cmdCart.Parameters.AddWithValue("@costShipping", costShipping);
            sqlCommands.Add(cmdCart);

            strCommand.Clear();
            strCommand.Append($"INSERT INTO {sqlTableNameCartItems}");
            strCommand.Append($"(cartId, ownerId, itemName, itemLink, itemPrice, quantity) ");
            strCommand.Append($"VALUES ");
            strCommand.Append($"(@cartId, @ownerId, @itemName, @itemLink, @itemPrice, @quantity) ");
            foreach (var kvp in cartItems)
            {
                Simp simp = kvp.Key;
                foreach (var list in kvp.Value)
                {
                    Item item = list.Key;
                    uint quantity = list.Value;

                    SqlCommand cmdItem = new SqlCommand(strCommand.ToString());
                    cmdItem.Parameters.AddWithValue("@cartId", uDex);
                    cmdItem.Parameters.AddWithValue("@ownerId", simp.uDex);
                    cmdItem.Parameters.AddWithValue("@itemName", item.itemName);
                    cmdItem.Parameters.AddWithValue("@itemLink", item.itemLink);
                    cmdItem.Parameters.AddWithValue("@itemPrice", item.priceSGD);
                    cmdItem.Parameters.AddWithValue("@quantity", quantity);
                    sqlCommands.Add(cmdItem);
                }
            }
        }
        public static void Deserialize(IDataReader reader)
        {
            Cart cart = new Cart(
                cartName: reader.GetCastedValueOrDefault("cartName", string.Empty),
                cartOwner: Simp.GetSimp(reader.GetCastedValueOrDefault("ownerId", uint.MaxValue)),
                cartDateStart: reader.GetCastedValueOrDefault("cartDateStart", DateTime.Now),
                cartDatePlan: reader.GetCastedValueOrDefault("cartDatePlan", DateTime.Now.AddDays(Consts.defaultCartPlan))
                );

            DateTime cartDateEnd = reader.GetCastedValueOrDefault("cartDateEnd", DateTime.Now.AddTicks(-1));
            double shippingCost = reader.GetCastedValueOrDefault("shippingCost", 0.0);

            if (cartDateEnd >= cart.cartDateStart)
                cart.closeCart(cartDateEnd);
            cart.setShippingCost(shippingCost);
        }
        public static void DeserializeCartItems(IDataReader reader)
        {
            uint cartId = reader.GetCastedValueOrDefault("cartId", uint.MaxValue);
            Simp simp = Simp.GetSimp(reader.GetCastedValueOrDefault("ownerId", uint.MaxValue));
            Item item = new(
                itemName: reader.GetCastedValueOrDefault("itemName", string.Empty),
                itemLink: reader.GetCastedValueOrDefault("itemLink", string.Empty),
                priceSGD: reader.GetCastedValueOrDefault("itemPrice", 0.0)
                );
            uint quantity = reader.GetCastedValueOrDefault("quantity", 1u);
            uDexCarts[cartId].addItem(simp, item, quantity);
        }
        public static void DeserializeAll(SqlConnection connection)
        {
            SqlCommand cmd;
            cmd = new SqlCommand($"SELECT * FROM {sqlTableName}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    Deserialize(reader);

            cmd = new SqlCommand($"SELECT * FROM {sqlTableNameCartItems}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    DeserializeCartItems(reader);
        }
        #endregion
    }
}