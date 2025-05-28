using Npgsql;
using System.Data;
using System.Text;

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

        public Cart(
            string cartName, Simp cartOwner,
            DateTime? cartDateStart = null, DateTime? cartDatePlan = null)
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
            CartDateEnd = DateTime.MinValue;
            CostShipping = 0.0;

            cartItems = new Dictionary<Simp, Dictionary<Item, uint>>();

            //-+-+-+-+-+-+-+-+
            // Enter to Database
            //-+-+-+-+-+-+-+-+
            StringBuilder sqlCmdStr = new();
            sqlCmdStr.AppendLine($@"INSERT INTO {DbHandler.sqlTableCarts} (");
            sqlCmdStr.AppendLine($@"    u_dex,");
            sqlCmdStr.AppendLine($@"    cart_name,");
            sqlCmdStr.AppendLine($@"    owner_id,");
            sqlCmdStr.AppendLine($@"    cart_date_start,");
            sqlCmdStr.AppendLine($@"    cart_date_plan,");
            sqlCmdStr.AppendLine($@"    cart_date_end,");
            sqlCmdStr.AppendLine($@"    cost_shipping");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"VALUES (");
            sqlCmdStr.AppendLine($@"    @uDex,");
            sqlCmdStr.AppendLine($@"    @cartName,");
            sqlCmdStr.AppendLine($@"    @ownerId,");
            sqlCmdStr.AppendLine($@"    @cartDateStart,");
            sqlCmdStr.AppendLine($@"    @cartDatePlan,");
            sqlCmdStr.AppendLine($@"    @cartDateEnd,");
            sqlCmdStr.AppendLine($@"    @costShipping");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"ON CONFLICT (u_dex) DO UPDATE SET");
            sqlCmdStr.AppendLine($@"    cart_name = EXCLUDED.cart_name,");
            sqlCmdStr.AppendLine($@"    owner_id = EXCLUDED.owner_id,");
            sqlCmdStr.AppendLine($@"    cart_date_start = EXCLUDED.cart_date_start,");
            sqlCmdStr.AppendLine($@"    cart_date_plan = EXCLUDED.cart_date_plan,");
            sqlCmdStr.AppendLine($@"    cart_date_end = EXCLUDED.cart_date_end,");
            sqlCmdStr.AppendLine($@"    cost_shipping = EXCLUDED.cost_shipping;");
            var sqlCmd = new NpgsqlCommand(sqlCmdStr.ToString());
            sqlCmd.Parameters.AddWithValue("@uDex", uDex);
            sqlCmd.Parameters.AddWithValue("@cartName", cartName);
            sqlCmd.Parameters.AddWithValue("@ownerId", cartOwner.uDex);
            sqlCmd.Parameters.AddWithValue("@cartDateStart", CartDateStart);
            sqlCmd.Parameters.AddWithValue("@cartDatePlan", CartDatePlan);
            sqlCmd.Parameters.AddWithValue("@cartDateEnd", cartDateEnd);
            sqlCmd.Parameters.AddWithValue("@costShipping", costShipping);
            Task.Run(() => DbHandler.RunSqlCommand(sqlCmd));
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
        public void setShippingCost(double costShipping)
        {
            CostShipping = costShipping;
            
            //-+-+-+-+-+-+-+-+
            // Enter to Database
            //-+-+-+-+-+-+-+-+
            StringBuilder sqlCmdStr = new();
            sqlCmdStr.AppendLine($@"UPDATE {DbHandler.sqlTableCarts} SET");
            sqlCmdStr.AppendLine($@"    cost_shipping = @costShipping");
            sqlCmdStr.AppendLine($@"WHERE u_dex = @uDex;");
            var sqlCmd = new NpgsqlCommand(sqlCmdStr.ToString());
            sqlCmd.Parameters.AddWithValue("@uDex", uDex);
            sqlCmd.Parameters.AddWithValue("@costShipping", costShipping);
            Task.Run(() => DbHandler.RunSqlCommand(sqlCmd));
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
            
            //-+-+-+-+-+-+-+-+
            // Enter to Database
            //-+-+-+-+-+-+-+-+
            StringBuilder sqlCmdStr = new();
            sqlCmdStr.AppendLine($@"UPDATE {DbHandler.sqlTableCarts} SET");
            sqlCmdStr.AppendLine($@"    cart_date_end = @cartDateEnd");
            sqlCmdStr.AppendLine($@"WHERE u_dex = @uDex;");
            var sqlCmd = new NpgsqlCommand(sqlCmdStr.ToString());
            sqlCmd.Parameters.AddWithValue("@uDex", uDex);
            sqlCmd.Parameters.AddWithValue("@cartDateEnd", cartDateEnd);
            Task.Run(() => DbHandler.RunSqlCommand(sqlCmd));
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
        public bool changeItemQuantity(Simp simp, string itemName, int quantity = 1)
        {
            if (!stillOpen)
                return false;

            Dictionary<Item, uint> simpItems;

            // simp does not have items in the cart
            if (!cartItems.TryGetValue(simp, out simpItems))
                return false;

            bool itemFound = false;
            bool isNowEmpty = false;
            foreach (var item in simpItems.Keys)
            {
                if (item.itemName == itemName)
                {
                    itemFound = true;
                    simpItems[item] = (uint)quantity;
                    if (quantity <= 0)
                    {
                        simpItems.Remove(item);
                        if (simpItems.Count == 0)
                        {
                            isNowEmpty = true;
                            cartItems.Remove(simp);
                        }
                    }
                    break;
                }
            }

            if (itemFound)
            {
                StringBuilder sqlCmdStr = new();
                NpgsqlCommand sqlCmd;
                if (isNowEmpty)
                {
                    sqlCmdStr.AppendLine($@"DELETE FROM {DbHandler.sqlTableCartItems} WHERE cart_id = @uDex AND simp_id = @simpId;");
                    sqlCmd = new NpgsqlCommand(sqlCmdStr.ToString());
                }
                else
                {
                    Item[] listItem = simpItems.Keys.ToArray();
                    int[] listQuantity = simpItems.Values.Select(x => (int)x).ToArray();

                    sqlCmdStr.AppendLine($@"UPDATE {DbHandler.sqlTableCartItems} SET");
                    sqlCmdStr.AppendLine($@"    cart_id = @uDex,");
                    sqlCmdStr.AppendLine($@"    simp_id = @simpId,");
                    sqlCmdStr.AppendLine($@"    items = @items,");
                    sqlCmdStr.AppendLine($@"    quantities = @quantities");
                    sqlCmdStr.AppendLine($@"WHERE cart_id = @uDex AND simp_id = @simpId;");
                    sqlCmd = new NpgsqlCommand(sqlCmdStr.ToString());
                    sqlCmd.Parameters.AddWithValue("@uDex", uDex);
                    sqlCmd.Parameters.AddWithValue("@simpId", simp.uDex);
                    sqlCmd.Parameters.AddWithValue("@items", listItem);
                    sqlCmd.Parameters.AddWithValue("@quantities", listQuantity);
                }
                Task.Run(() => DbHandler.RunSqlCommand(sqlCmd));

                return true;
            }
            
            return false;
        }
        public bool addItem(Simp simp, Item item, int quantity = 1, bool force = false)
        {
            if(!stillOpen && !force)
                return false;
            
            Dictionary<Item, uint> simpItems;
            if (!cartItems.TryGetValue(simp, out simpItems))
            {
                simpItems = new();
                cartItems.Add(simp, simpItems);
            }
            simpItems.AddFrequency(item, (uint)quantity);

            //-+-+-+-+-+-+-+-+
            // Enter to Database
            //-+-+-+-+-+-+-+-+
            Item[] listItem = simpItems.Keys.ToArray();
            int[] listQuantity = simpItems.Values.Select(x => (int)x).ToArray();
            StringBuilder sqlCmdStr = new();
            sqlCmdStr.AppendLine($@"INSERT INTO {DbHandler.sqlTableCartItems} (");
            sqlCmdStr.AppendLine($@"    cart_id,");
            sqlCmdStr.AppendLine($@"    simp_id,");
            sqlCmdStr.AppendLine($@"    items,");
            sqlCmdStr.AppendLine($@"    quantities");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"VALUES (");
            sqlCmdStr.AppendLine($@"    @uDex,");
            sqlCmdStr.AppendLine($@"    @simpId,");
            sqlCmdStr.AppendLine($@"    @items,");
            sqlCmdStr.AppendLine($@"    @quantities");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"ON CONFLICT (cart_id, simp_id) DO UPDATE SET");
            sqlCmdStr.AppendLine($@"    items = EXCLUDED.items,");
            sqlCmdStr.AppendLine($@"    quantities = EXCLUDED.quantities;");
            var sqlCmd = new NpgsqlCommand(sqlCmdStr.ToString());
            sqlCmd.Parameters.AddWithValue("@uDex", uDex);
            sqlCmd.Parameters.AddWithValue("@simpId", simp.uDex);
            sqlCmd.Parameters.AddWithValue("@items", listItem);
            sqlCmd.Parameters.AddWithValue("@quantities", listQuantity);
            Task.Run(() => DbHandler.RunSqlCommand(sqlCmd));

            return true;
        }
        #endregion

        //-+-+-+-+-+-+-+-+-+
        // Database
        //-+-+-+-+-+-+-+-+-+
        #region Database
        const string sqlTableName = "Carts";
        const string sqlTableNameCartItems = "CartItems";
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
            StringBuilder sqlCmdStr = new();
            //-+-+-+-+-+-+-+-+
            // Create Table if not exists
            //-+-+-+-+-+-+-+-+
            sqlCmdStr.Clear();
            sqlCmdStr.AppendLine($@"INSERT INTO {DbHandler.sqlTableCarts} (");
            sqlCmdStr.AppendLine($@"    u_dex,");
            sqlCmdStr.AppendLine($@"    cart_name,");
            sqlCmdStr.AppendLine($@"    owner_id,");
            sqlCmdStr.AppendLine($@"    cart_date_start,");
            sqlCmdStr.AppendLine($@"    cart_date_plan,");
            sqlCmdStr.AppendLine($@"    cart_date_end,");
            sqlCmdStr.AppendLine($@"    cost_shipping");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"VALUES (");
            sqlCmdStr.AppendLine($@"    @uDex,");
            sqlCmdStr.AppendLine($@"    @cartName,");
            sqlCmdStr.AppendLine($@"    @ownerId,");
            sqlCmdStr.AppendLine($@"    @cartDateStart,");
            sqlCmdStr.AppendLine($@"    @cartDatePlan,");
            sqlCmdStr.AppendLine($@"    @cartDateEnd,");
            sqlCmdStr.AppendLine($@"    @costShipping");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"ON CONFLICT (u_dex) DO UPDATE SET");
            sqlCmdStr.AppendLine($@"    cart_name = EXCLUDED.cart_name,");
            sqlCmdStr.AppendLine($@"    owner_id = EXCLUDED.owner_id,");
            sqlCmdStr.AppendLine($@"    cart_date_start = EXCLUDED.cart_date_start,");
            sqlCmdStr.AppendLine($@"    cart_date_plan = EXCLUDED.cart_date_plan,");
            sqlCmdStr.AppendLine($@"    cart_date_end = EXCLUDED.cart_date_end,");
            sqlCmdStr.AppendLine($@"    cost_shipping = EXCLUDED.cost_shipping;");
            var insertCommand = new NpgsqlCommand(sqlCmdStr.ToString());
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
            sqlCmdStr.Clear();
            foreach(var kvp in cartItems)
            {
                Simp simp = kvp.Key;
                var changeItemQuantity = kvp.Value;

                var listItem = changeItemQuantity.Keys.ToArray();
                var listQuantity = changeItemQuantity.Values.Select(x => (int)x).ToArray();
                
                sqlCmdStr.AppendLine($@"INSERT INTO {DbHandler.sqlTableCartItems} (");
                sqlCmdStr.AppendLine($@"    cart_id,");
                sqlCmdStr.AppendLine($@"    simp_id,");
                sqlCmdStr.AppendLine($@"    items,");
                sqlCmdStr.AppendLine($@"    quantities");
                sqlCmdStr.AppendLine($@")");
                sqlCmdStr.AppendLine($@"VALUES (");
                sqlCmdStr.AppendLine($@"    @uDex,");
                sqlCmdStr.AppendLine($@"    @simpId,");
                sqlCmdStr.AppendLine($@"    @items,");
                sqlCmdStr.AppendLine($@"    @quantities");
                sqlCmdStr.AppendLine($@")");
                sqlCmdStr.AppendLine($@"ON CONFLICT (cart_id, simp_id) DO UPDATE SET");
                sqlCmdStr.AppendLine($@"    items = EXCLUDED.items,");
                sqlCmdStr.AppendLine($@"    quantities = EXCLUDED.quantities;");
                var insertItemCommand = new NpgsqlCommand(sqlCmdStr.ToString());
                insertItemCommand.Parameters.AddWithValue("@uDex", uDex);
                insertItemCommand.Parameters.AddWithValue("@simpId", simp.uDex);
                var itemsParam = insertItemCommand.Parameters.AddWithValue("@items", listItem.ToArray());
                insertItemCommand.Parameters.AddWithValue("@quantities", listQuantity.ToArray());

                sqlCommands.Add(insertItemCommand);
            }
            //-+-+-+-+-+-+-+-+
        }
        public static void Deserialize(IDataReader reader)
        {
            Cart cart = new Cart(
                cartName: reader.GetCastedValueOrDefault("cart_name", string.Empty),
                cartOwner: Simp.GetSimp(reader.GetCastedValueOrDefault("owner_id", -1)),
                cartDateStart: reader.GetCastedValueOrDefault("cart_date_start", DateTime.Now),
                cartDatePlan: reader.GetCastedValueOrDefault("cart_date_plan", DateTime.Now.AddDays(Consts.defaultCartPlan))
                );

            double shippingCost = reader.GetCastedValueOrDefault("cost_shipping", 0.0);
            cart.setShippingCost(shippingCost);
        }
        public static void DeserializeDateEnd(IDataReader reader)
        {
            int uDex = reader.GetCastedValueOrDefault("u_dex", int.MaxValue);
            DateTime cartDateEnd = reader.GetCastedValueOrDefault("cart_date_end", DateTime.MinValue);
            uDexCarts[uDex].closeCart(cartDateEnd);
        }
        public static void DeserializeCartItems(IDataReader reader)
        {
            int cartId = reader.GetCastedValueOrDefault("cart_id", -1);
            Simp simp = Simp.GetSimp(reader.GetCastedValueOrDefault("owner_id", -1));
            Item item = new(
                itemName: reader.GetCastedValueOrDefault("item_name", string.Empty),
                itemLink: reader.GetCastedValueOrDefault("item_link", string.Empty),
                priceSGD: reader.GetCastedValueOrDefault("item_price", 0.0)
                );
            int quantity = reader.GetCastedValueOrDefault("quantity", 1);
            uDexCarts[cartId].addItem(simp, item, quantity);
        }
        public static void DeserializeAll(NpgsqlConnection connection)
        {
            NpgsqlCommand cmd;
            cmd = new NpgsqlCommand($"SELECT * FROM {DbHandler.sqlTableCarts}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    Deserialize(reader);

            cmd = new NpgsqlCommand($"SELECT * FROM {DbHandler.sqlTableCartItems}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    DeserializeCartItems(reader);
        }
        #endregion
    }
}