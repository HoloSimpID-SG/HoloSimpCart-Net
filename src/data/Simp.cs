using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HoloSimpID
{
    public class Simp
    {
        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        #region Indexer
        private static uint indexer = 0;
        public uint uDex => UDex; private readonly uint UDex;

        private static readonly Dictionary<uint, Simp> uDexSimps = new();
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
        }

        //-+-+-+-+-+-+-+-+-+
        // Instance Getter
        //-+-+-+-+-+-+-+-+-+
        #region Instance Getter
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the cart with the <see cref="uDex"/> <paramref name="cartId"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static Simp GetSimp(uint cartId)
        {
            if (uDexSimps.TryGetValue(cartId, out var simp))
                return simp;
            return null;
        }

        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the first <see cref="Simp"/> with <see cref="Simp.name"/> that matches the <paramref name="cartName"/>.
        /// <br/> - Prioritize open carts.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static Simp GetSimp(string simpName)
        {
            var simps = uDexSimps.Where(x => x.Value.simpName == simpName).Select(x => x.Value);
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
        public static void GetAllSimps(IList<Simp> simps, Func<uint, IDictionary<uint, Simp>, bool> predicate = null)
        {
            predicate ??= (i, l) => l[i] != null;
            int len = uDexSimps.Count;
            for (uint i = 0; i < len; i++)
                if (!predicate(i, uDexSimps)) simps.Add(uDexSimps[i]);
        }

        /// <summary>
        /// <inheritdoc cref="GetAllSimps"/>
        /// </summary>
        public static List<Simp> GetAllSimps(Func<uint, IDictionary<uint, Simp>, bool> predicate = null)
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
        public static List<SqlCommand> SerializeAll()
        {
            List<SqlCommand> sqlCommands = new();
            SerializeAll(sqlCommands);
            return sqlCommands;
        }
        public static void SerializeAll(IList<SqlCommand> sqlCommands)
        {
            foreach (Simp simp in uDexSimps.Values)
                simp.Serialize(sqlCommands);
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
            strCommand.Append($"dcUserName {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($"simpName {MoLibrary.sqlDataType[typeof(string)]}, ");
            strCommand.Append($")");
            var cmdTable = new SqlCommand(strCommand.ToString());
            sqlCommands.Add(cmdTable);
            strCommand.Clear();
            strCommand.Append($"CREATE TABLE IF NOT EXISTS {sqlTableNameSpendMerch}");
            strCommand.Append($"(");
            strCommand.Append($"simpId {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($"value {MoLibrary.sqlDataType[typeof(double)]}, ");
            strCommand.Append($"frequency {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($")");
            var cmdTableMerch = new SqlCommand(strCommand.ToString());
            sqlCommands.Add(cmdTableMerch);
            strCommand.Clear();
            strCommand.Append($"CREATE TABLE IF NOT EXISTS {sqlTableNameSpendMisc}");
            strCommand.Append($"(");
            strCommand.Append($"simpId {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($"value {MoLibrary.sqlDataType[typeof(double)]}, ");
            strCommand.Append($"frequency {MoLibrary.sqlDataType[typeof(uint)]}, ");
            strCommand.Append($")");
            var cmdTableMisc = new SqlCommand(strCommand.ToString());
            sqlCommands.Add(cmdTableMisc);
            //-+-+-+-+-+-+-+-+

            strCommand.Clear();
            strCommand.Append($"INSERT INTO {sqlTableName}");
            strCommand.Append($"(dcUserName, simpName) ");
            strCommand.Append($"VALUES ");
            strCommand.Append($"(@dcUserName, @simpName) ");
            var cmdSimp = new SqlCommand(strCommand.ToString());
            cmdSimp.Parameters.AddWithValue("@dcUserName", dcUserName);
            cmdSimp.Parameters.AddWithValue("@simpName", simpName);
            sqlCommands.Add(cmdSimp);

            sqlCommands.Add(cmdTable);
            strCommand.Clear();
            strCommand.Append($"INSERT INTO {sqlTableNameSpendMerch}");
            strCommand.Append($"(simpId, value, frequency) ");
            strCommand.Append($"VALUES ");
            strCommand.Append($"(@simpId, @value, @frequency) ");
            foreach (var kvp in merchSpending)
            {
                double value = kvp.Key;
                uint freq = kvp.Value;

                var cmdItem = new SqlCommand(strCommand.ToString());
                cmdItem.Parameters.AddWithValue("@simpId", uDex);
                cmdItem.Parameters.AddWithValue("@value", value);
                cmdItem.Parameters.AddWithValue("@frequency", freq);
                sqlCommands.Add(cmdItem);
            }

            strCommand.Clear();
            strCommand.Append($"INSERT INTO {sqlTableNameSpendMisc}");
            strCommand.Append($"(simpId, value, frequency) ");
            strCommand.Append($"VALUES ");
            strCommand.Append($"(@simpId, @value, @frequency) ");
            foreach (var kvp in miscSpending)
            {
                double value = kvp.Key;
                uint freq = kvp.Value;

                var cmdItem = new SqlCommand(strCommand.ToString());
                cmdItem.Parameters.AddWithValue("@simpId", uDex);
                cmdItem.Parameters.AddWithValue("@value", value);
                cmdItem.Parameters.AddWithValue("@frequency", freq);
                sqlCommands.Add(cmdItem);
            }
        }
        public static Simp Deserialize(IDataReader reader)
        {
            return new Simp(
                dcUserName: reader.GetCastedValueOrDefault("dcUserName", string.Empty),
                simpName: reader.GetCastedValueOrDefault("simpName", string.Empty)
                );
        }
        public static void DeserializeMerchSpending(IDataReader reader)
        {
            uint simpId = reader.GetCastedValueOrDefault("simpId", uint.MaxValue);
            uDexSimps[simpId].addMerchSpending(
                reader.GetCastedValueOrDefault("value", 0.0),
                reader.GetCastedValueOrDefault("freq", 1u)
                );
        }
        public static void DeserializeMiscSpending(IDataReader reader)
        {
            uint simpId = reader.GetCastedValueOrDefault("simpId", uint.MaxValue);
            uDexSimps[simpId].addMiscSpending(
                reader.GetCastedValueOrDefault("value", 0.0),
                reader.GetCastedValueOrDefault("freq", 1u)
                );
        }
        public static void DeserializeAll(SqlConnection connection)
        {
            SqlCommand cmd;
            cmd = new SqlCommand ($"SELECT * FROM {sqlTableName}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    Deserialize(reader);

            cmd = new SqlCommand ($"SELECT * FROM {sqlTableNameSpendMerch}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    DeserializeMerchSpending(reader);

            cmd = new SqlCommand ($"SELECT * FROM {sqlTableNameSpendMisc}", connection);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    DeserializeMiscSpending(reader);
        }
        #endregion
    }
}