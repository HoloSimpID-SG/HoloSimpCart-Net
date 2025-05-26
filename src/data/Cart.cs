using System.Data;
using System.Text;
using Npgsql;

namespace HoloSimpID
{
    public class Cart
    {
        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        #region Indexer
        private static int indexer = 0;
        public int uDex => UDex; private readonly int UDex;

        private static readonly Dictionary<int, Cart> uDexCarts = new();
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
        public Dictionary<Simp, Dictionary<Item, uint>> cartItems { get; private set; }
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
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Fills a <see cref="IList{T}"/> of <see cref="Cart"/> that fullfils the <paramref name="predicate"/>.
        /// <br/> - <paramref name="predicate"/> takes the <see langword="uint"/> for the index to access the <see cref="IDictionary{TKey, TValue}"/>.
        /// <br/> - The <see cref="IDictionary{TKey, TValue}"/> will take <see cref="uDexCarts"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static void GetAllCarts(IList<Cart> carts, Func<int, IDictionary<int, Cart>, bool> predicate = null)
        {
            predicate ??= (i, l) => l[i] != null;
            int len = uDexCarts.Count;
            for (int i = 0; i < len; i++)
                if (predicate(i, uDexCarts)) carts.Add(uDexCarts[i]);
        }
        /// <summary>
        /// <inheritdoc cref="GetAllCarts"/>
        /// </summary>
        public static List<Cart> GetAllCarts(Func<int, IDictionary<int, Cart>, bool> predicate = null)
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
        public void setShippingCost(double shippingCost)
        {
            CostShipping = shippingCost;
        }
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
            strResult.AppendLine($"Owned by: {cartOwner.simpName}");
            strResult.Append($"Status: ");
            strResult.AppendLine(stillOpen ? $"Open" : "Closed");
            strResult.AppendLine($"Opened at: {cartDateStart}");
            strResult.AppendLine($"Item List:");
            foreach (var kvp in cartItems)
            {
                Simp simp = kvp.Key;
                strResult.AppendLine($"- {simp}:");
                foreach (var itemQuantityPair in kvp.Value)
                    strResult.AppendLine($"  - {itemQuantityPair.Key} ({itemQuantityPair.Key.priceSGD:C2}) {Consts.cMultiply}{itemQuantityPair.Value}");
            }
            return strResult.ToString();
        }
        public bool addItem(Simp simp, Item item, int quantity = 1)
        {
            if (stillOpen)
            {
                Dictionary<Item, uint> simpItems;
                if (!cartItems.TryGetValue(simp, out simpItems))
                {
                    simpItems = new();
                    cartItems.Add(simp, simpItems);
                }
                simpItems.AddFrequency(item, (uint)quantity);
                return true;
            }
            return false;
        }
        #endregion

        //-+-+-+-+-+-+-+-+-+
        // Database
        //-+-+-+-+-+-+-+-+-+
        #region Database
        public static List<NpgsqlCommand> SerializeAll()
        {
            List<NpgsqlCommand> sqlCommands = new();
            SerializeAll(sqlCommands);
            return sqlCommands;
        }
        public static void SerializeAll(IList<NpgsqlCommand> sqlCommands)
        {
            foreach (Cart cart in uDexCarts.Values)
                cart.Serialize(sqlCommands);
        }
        public List<NpgsqlCommand> Serialize()
        {
            List<NpgsqlCommand> sqlCommands = new();
            Serialize(sqlCommands);
            return sqlCommands;
        }
        public void Serialize(IList<NpgsqlCommand> sqlCommands)
        {
            StringBuilder strCommand = new();
            //-+-+-+-+-+-+-+-+
            // Cart Table
            //-+-+-+-+-+-+-+-+
            // Safe Create
            strCommand.Clear();
            strCommand.Append($@"CREATE TABLE IF NOT EXISTS {MoLibrary.sqlTableCarts} (");
            strCommand.Append($@"    u_dex SERIAL PRIMARY KEY,");
            strCommand.Append($@"    cart_name TEXT NOT NULL,");
            strCommand.Append($@"    owner_id INT REFERENCES {MoLibrary.sqlTableSimps}(u_dex),");
            strCommand.Append($@"    cart_date_start TIMESTAMPTZ,");
            strCommand.Append($@"    cart_date_plan TIMESTAMPTZ,");
            strCommand.Append($@"    cart_date_end TIMESTAMPTZ,");
            strCommand.Append($@"    cost_shipping DOUBLE PRECISION");
            strCommand.Append($@");");
            sqlCommands.Add(new NpgsqlCommand(strCommand.ToString()));

            // Insert
            strCommand.Clear();
            strCommand.Append($@"INSERT INTO {MoLibrary.sqlTableCarts} (");
            strCommand.Append($@"    u_dex,");
            strCommand.Append($@"    cart_name,");
            strCommand.Append($@"    owner_id,");
            strCommand.Append($@"    cart_date_start,");
            strCommand.Append($@"    cart_date_plan,");
            strCommand.Append($@"    cart_date_end,");
            strCommand.Append($@"    cost_shipping");
            strCommand.Append($@")");
            strCommand.Append($@"VALUES (");
            strCommand.Append($@"    @uDex,");
            strCommand.Append($@"    @cartName,");
            strCommand.Append($@"    @ownerId,");
            strCommand.Append($@"    @cartDateStart,");
            strCommand.Append($@"    @cartDatePlan,");
            strCommand.Append($@"    @cartDateEnd,");
            strCommand.Append($@"    @costShipping");
            strCommand.Append($@")");
            strCommand.Append($@"ON CONFLICT (u_dex) DO UPDATE SET");
            strCommand.Append($@"    cart_name = EXCLUDED.cartName,");
            strCommand.Append($@"    owner_id = EXCLUDED.ownerId,");
            strCommand.Append($@"    cart_date_start = EXCLUDED.cart_date_start,");
            strCommand.Append($@"    cart_date_plan = EXCLUDED.cart_date_plan,");
            strCommand.Append($@"    cart_date_end = EXCLUDED.cart_date_end,");
            strCommand.Append($@"    cost_shipping = EXCLUDED.cost_shipping;");
            var insertCommand = new NpgsqlCommand(strCommand.ToString());
            insertCommand.Parameters.AddWithValue("@uDex", uDex);
            insertCommand.Parameters.AddWithValue("@cartName", cartName);
            insertCommand.Parameters.AddWithValue("@ownerId", cartOwner.uDex);
            insertCommand.Parameters.AddWithValue("@cartDateStart", cartDateStart);
            insertCommand.Parameters.AddWithValue("@cartDatePlan", cartDatePlan);
            insertCommand.Parameters.AddWithValue("@cartDateEnd", cartDateEnd);
            insertCommand.Parameters.AddWithValue("@costShipping", costShipping);
            sqlCommands.Add(insertCommand);
            //-+-+-+-+-+-+-+-+


            //-+-+-+-+-+-+-+-+
            // Cart Items
            //-+-+-+-+-+-+-+-+
            // Safe Create
            strCommand.Clear();
            strCommand.Append($@"CREATE TABLE IF NOT EXISTS {MoLibrary.sqlTableCartItems} (");
            strCommand.Append($@"    cart_id INT REFERENCES {MoLibrary.sqlTableCarts}(u_dex),");
            strCommand.Append($@"    simp_id INT REFERENCES {MoLibrary.sqlTableSimps}(uDex),");
            strCommand.Append($@"    items item_type[],");
            strCommand.Append($@"    quantities INT[],");
            strCommand.Append($@"    PRIMARY KEY (cart_id, simp_id)");
            strCommand.Append($@");");
            sqlCommands.Add(new NpgsqlCommand(strCommand.ToString()));

            // Insert
            strCommand.Clear();
            foreach(var kvp in cartItems)
            {
                Simp simp = kvp.Key;
                List<Item> listItem = new();
                List<uint> listQuantity = new();
                foreach (var itemQuantityPair in kvp.Value)
                {
                    listItem.Add(itemQuantityPair.Key);
                    listQuantity.Add(itemQuantityPair.Value);
                }
                strCommand.Append($@"INSERT INTO {MoLibrary.sqlTableCartItems} (");
                strCommand.Append($@"    cart_id,");
                strCommand.Append($@"    simp_id,");
                strCommand.Append($@"    items,");
                strCommand.Append($@"    quantities");
                strCommand.Append($@")");
                strCommand.Append($@"VALUES (");
                strCommand.Append($@"    @uDex,");
                strCommand.Append($@"    @simpId,");
                strCommand.Append($@"    @items,");
                strCommand.Append($@"    @quantities");
                strCommand.Append($@")");
                strCommand.Append($@"ON CONFLICT (cart_id, simp_id) DO UPDATE SET");
                strCommand.Append($@"    items = EXCLUDED.items,");
                strCommand.Append($@"    quantities = EXCLUDED.quantities;");
                var insertItemCommand = new NpgsqlCommand(strCommand.ToString());
                insertItemCommand.Parameters.AddWithValue("@uDex", uDex);
                insertItemCommand.Parameters.AddWithValue("@simpId", simp.uDex);
                insertItemCommand.Parameters.AddWithValue("@items", listItem.ToArray());
                insertItemCommand.Parameters.AddWithValue("@quantities", listQuantity.ToArray());

                sqlCommands.Add(insertItemCommand);
            }
            //-+-+-+-+-+-+-+-+
        }
        public static void Deserialize(IDataReader reader)
        {
            Cart cart = new Cart(
                cartName: reader.GetCastedValueOrDefault("cart_name", string.Empty),
                cartOwner: Simp.GetSimp(reader.GetCastedValueOrDefault("owner_id", int.MaxValue)),
                cartDateStart: reader.GetCastedValueOrDefault("cart_date_start", DateTime.Now),
                cartDatePlan: reader.GetCastedValueOrDefault("cart_date_plan", DateTime.Now.AddDays(Consts.defaultCartPlan))
                );

            DateTime cartDateEnd = reader.GetCastedValueOrDefault("cart_date_end", DateTime.Now.AddTicks(-1));
            double shippingCost = reader.GetCastedValueOrDefault("cost_shipping", 0.0);

            if (cartDateEnd >= cart.cartDateStart)
                cart.closeCart(cartDateEnd);
            cart.setShippingCost(shippingCost);
        }
        public static void DeserializeCartItems(IDataReader reader)
        {
            int cartId = reader.GetCastedValueOrDefault("cart_id", int.MaxValue);
            int simpId = reader.GetCastedValueOrDefault("simp_id", int.MaxValue);

            Simp simp = Simp.GetSimp(simpId);
            var items = reader["items"] as Item[];
            var quantities = reader["quantities"] as int[];

            int len = items?.Length ?? 0;
            for (int i = 0; i < len; i++)
            {
                Item item = items[i];
                int quantity = (quantities?[i] ?? 1);
                uDexCarts[cartId].addItem(simp, item, quantity);
            }
        }
        public static void DeserializeAll(NpgsqlConnection connection)
        {
            NpgsqlCommand cmd;
            cmd = new NpgsqlCommand($"SELECT * FROM {MoLibrary.sqlTableCarts}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    Deserialize(reader);

            cmd = new NpgsqlCommand($"SELECT * FROM {MoLibrary.sqlTableCartItems}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    DeserializeCartItems(reader);
        }
        #endregion
    }
}