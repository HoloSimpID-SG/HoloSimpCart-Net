using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;

namespace HoloSimpID
{
    public class Simp
    {
        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        #region Indexer
        private static int indexer = 0;
        public int uDex => UDex; private readonly int UDex;

        private static readonly Dictionary<int, Simp> uDexSimps = new();
        private static readonly Dictionary<string, Simp> uGuidSimps = new();
        //-+-+-+-+-+-+-+-+
        #endregion

        public string dcUserName;
        public string simpName;
        public Dictionary<double, uint> merchSpending;
        public Dictionary<double, uint> miscSpending;

        public Simp(string dcUserName, string simpName = null)
        {
            //-+-+-+-+-+-+-+-+
            // Indexer
            //-+-+-+-+-+-+-+-+
            #region Indexer
            UDex = indexer++;
            uDexSimps.Add(uDex, this);
            uGuidSimps.Add(dcUserName, this);
            //-+-+-+-+-+-+-+-+
            #endregion

            this.dcUserName = dcUserName;
            this.simpName = simpName ?? dcUserName;
            merchSpending = new();
            miscSpending = new();
            
            //-+-+-+-+-+-+-+-+
            // Insert into Database
            //-+-+-+-+-+-+-+-+
            StringBuilder sqlCmdStr = new();

            sqlCmdStr.Clear();
            sqlCmdStr.AppendLine($@"INSERT INTO {DbHandler.sqlTableSimps} (");
            sqlCmdStr.AppendLine($@"    u_dex,");
            sqlCmdStr.AppendLine($@"    dc_user_name,");
            sqlCmdStr.AppendLine($@"    simp_name");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"VALUES (");
            sqlCmdStr.AppendLine($@"    @uDex,");
            sqlCmdStr.AppendLine($@"    @dcUserName,");
            sqlCmdStr.AppendLine($@"    @simpName");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"ON CONFLICT (u_dex) DO UPDATE SET");
            sqlCmdStr.AppendLine($@"    dc_user_name = EXCLUDED.dc_user_name,");
            sqlCmdStr.AppendLine($@"    simp_name = EXCLUDED.simp_name;");
            var sqlCmd = new NpgsqlCommand(sqlCmdStr.ToString());
            sqlCmd.Parameters.AddWithValue("@uDex", uDex);
            sqlCmd.Parameters.AddWithValue("@dcUserName", dcUserName);
            sqlCmd.Parameters.AddWithValue("@simpName", simpName);
            
            Task.Run(() => DbHandler.RunSqlCommand(sqlCmd));
            //-+-+-+-+-+-+-+-+
        }

        //-+-+-+-+-+-+-+-+-+
        // Instance Getter
        //-+-+-+-+-+-+-+-+-+
        #region Instance Getter
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the cart with the <see cref="uDex"/> <paramref name="simpId"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static Simp? GetSimp(int simpId)
        {
            if (uDexSimps.TryGetValue(simpId, out var simp))
                return simp;
            return null;
        }

        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the first <see cref="Simp"/> with <see cref="Simp.name"/> that matches the <paramref name="cartName"/>.
        /// <br/> - Prioritize open carts.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static Simp? GetSimp(string simpName)
        {
            var simps = uDexSimps.Where(x => x.Value.dcUserName == simpName).Select(x => x.Value);
            if (simps.IsNullOrEmpty())
                return null;
            else
                return simps.First();
        }

        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Fills a <see cref="IList{T}"/> of <see cref="Simp"/> that fullfils the <paramref name="predicate"/>.
        /// <br/> - <paramref name="predicate"/> takes the <see langword="uint"/> for the index to access the <see cref="IDictionary{TKey, TValue}"/>.
        /// <br/> - The <see cref="IDictionary{TKey, TValue}"/> will take <see cref="uDexSimps"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static void GetAllSimps(IList<Simp> simps, Func<int, IDictionary<int, Simp>, bool> predicate = null)
        {
            predicate ??= (i, l) => l[i] != null;
            int len = uDexSimps.Count;
            for (int i = 0; i < len; i++)
                if (!predicate(i, uDexSimps)) simps.Add(uDexSimps[i]);
        }

        /// <summary>
        /// <inheritdoc cref="GetAllSimps"/>
        /// </summary>
        public static List<Simp> GetAllSimps(Func<int, IDictionary<int, Simp>, bool> predicate = null)
        {
            List<Simp> simps = new();
            GetAllSimps(simps, predicate);
            return simps;
        }
        //-+-+-+-+-+-+-+-+-+
        #endregion

        //-+-+-+-+-+-+-+-+-+
        // Simp Actions
        //-+-+-+-+-+-+-+-+-+
        #region Simp Actions
        public void addItemToHistory(Item item) => merchSpending.AddFrequency(item.priceSGD);
        public void addItemToHistory(KeyValuePair<Item, uint> itemQuantityPair) => merchSpending.AddFrequency(itemQuantityPair.Key.priceSGD, itemQuantityPair.Value);
        public void addMerchSpending(double value, uint freq = 1) => merchSpending.AddFrequency(value, freq);
        public void addMerchSpending(KeyValuePair<double, uint> valueFreqPair) => merchSpending.AddFrequency(valueFreqPair);
        public void addMiscSpending(double priceInSGD, uint freq = 1) => miscSpending.AddFrequency(priceInSGD);
        public void addMiscSpending(KeyValuePair<double, uint> valueFreqPair) => miscSpending.AddFrequency(valueFreqPair);
        public override string ToString() => simpName;
        #endregion

        //-+-+-+-+-+-+-+-+-+
        // Database
        //-+-+-+-+-+-+-+-+-+
        #region Database
        const string sqlTableName = "Simps";
        const string sqlTableNameSpendMerch = "SimpSpend";
        const string sqlTableNameSpendMisc = "SimpSpendMisc";
        public static List<NpgsqlCommand> SerializeAll()
        {
            List<NpgsqlCommand> sqlCommands = new();
            SerializeAll(sqlCommands);
            return sqlCommands;
        }
        public static void SerializeAll(IList<NpgsqlCommand> sqlCommands)
        {
            foreach (Simp simp in uDexSimps.Values)
                simp.Serialize(sqlCommands);
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

            sqlCmdStr.Clear();
            sqlCmdStr.AppendLine($@"INSERT INTO {DbHandler.sqlTableSimps} (");
            sqlCmdStr.AppendLine($@"    u_dex,");
            sqlCmdStr.AppendLine($@"    dc_user_name,");
            sqlCmdStr.AppendLine($@"    simp_name");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"VALUES (");
            sqlCmdStr.AppendLine($@"    @uDex,");
            sqlCmdStr.AppendLine($@"    @dcUserName,");
            sqlCmdStr.AppendLine($@"    @simpName");
            sqlCmdStr.AppendLine($@")");
            sqlCmdStr.AppendLine($@"ON CONFLICT (u_dex) DO UPDATE SET");
            sqlCmdStr.AppendLine($@"    dc_user_name = EXCLUDED.dc_user_name,");
            sqlCmdStr.AppendLine($@"    simp_name = EXCLUDED.simp_name;");
            var insertCommand = new NpgsqlCommand(sqlCmdStr.ToString());
            insertCommand.Parameters.AddWithValue("@uDex", uDex);
            insertCommand.Parameters.AddWithValue("@dcUserName", dcUserName);
            insertCommand.Parameters.AddWithValue("@simpName", simpName);
            sqlCommands.Add(insertCommand);
        }
        public static Simp Deserialize(IDataReader reader)
        {
            return new Simp(
                dcUserName: reader.GetCastedValueOrDefault("dcUserName", string.Empty),
                simpName: reader.GetCastedValueOrDefault("simpName", string.Empty)
                );
        }
        public static void DeserializeAll(NpgsqlConnection connection)
        {
            NpgsqlCommand cmd;
            cmd = new NpgsqlCommand ($"SELECT * FROM {sqlTableName}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    Deserialize(reader);
        }
        #endregion
    }
}