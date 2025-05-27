using Npgsql;
using System.Collections.Immutable;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace HoloSimpID
{
    public static partial class MoLibrary
    {
        public static string ToNpgsqlString(this string str) => '\'' + str.Replace("'", "''") + '\'';
        public static string ToNpgsqlDate(this DateTime dateTime) => '\'' + $"{dateTime:o}" + '\'';

        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{TKey, TValue}(IDictionary{TKey, object}, TKey, Func{object, TValue}, TValue)"/>
        /// </summary>
        public static T GetCastedValueOrDefault<T>(this IDataReader reader, string key, Func<object, T> castDefinition, T defaultValue = default)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == key)
                    return castDefinition(reader[i]);
            }
            return defaultValue;
        }
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{T}(IDataReader, string, Func{object, T}, T)"/>
        /// </summary>
        public static string GetCastedValueOrDefault(this IDataReader reader, string key, string defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => x as string, defaultValue);
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{T}(IDataReader, string, Func{object, T}, T)"/>
        /// </summary>
        public static DateTime GetCastedValueOrDefault(this IDataReader reader, string key, DateTime defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => x is DateTime dt ? dt : DateTime.Parse(x.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind), defaultValue);
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{T}(IDataReader, string, Func{object, T}, T)"/>
        /// </summary>
        public static uint GetCastedValueOrDefault(this IDataReader reader, string key, uint defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => Convert.ToUInt32(x), defaultValue);
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{T}(IDataReader, string, Func{object, T}, T)"/>
        /// </summary>
        public static int GetCastedValueOrDefault(this IDataReader reader, string key, int defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => Convert.ToInt32(x), defaultValue);
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{T}(IDataReader, string, Func{object, T}, T)"/>
        /// </summary>
        public static double GetCastedValueOrDefault(this IDataReader reader, string key, double defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => Convert.ToDouble(x), defaultValue);

        public static readonly ImmutableDictionary<Type, string> sqlDataType = new Dictionary<Type, string>()
        {
            { typeof(string), "TEXT" },
            { typeof(uint), "INT" },
            { typeof(int), "INT" },
            { typeof(double), "DOUBLE PRECISION" },
            { typeof(DateTime), "TIMESTAMPTZ" },
        }.ToImmutableDictionary();
        public const string sqlIndex = "uDex";
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns sets of <see cref="NpgsqlCommand"/> for safely upserting <paramref name="data"/>.
        /// <br/> - Automatically creates the table and column if it does not exist.
        /// <br/> - Automatically interprets the column name and type from <paramref name="dataName"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        /// <param name="dataName">
        /// <br/> - This one will automatically takes the variable name of <paramref name="data"/>.
        /// <br/> - Fill this to override.
        /// </param>
        public static List<NpgsqlCommand> SafeUpsert(
            string tableName, uint uDex,
            object data,
            [CallerArgumentExpression("data")] string dataName = null)
        {
            List<NpgsqlCommand> commands = new();
            StringBuilder strCommand = new();

            // Safe Conversion for DateTime
            if (data is DateTime dateTime)
                data = $"{dateTime:o}";
            // Get type AFTER DateTIme is converted
            Type type = data.GetType();

            //-+-+-+-+-+-+-+-+
            // Create Table if Not Exist
            //-+-+-+-+-+-+-+-+
            strCommand.Clear();
            strCommand.AppendLine($@"CREATE TABLE IF NOT EXISTS {tableName} (");
            strCommand.AppendLine($@"    uDex INT PRIMARY KEY");
            strCommand.AppendLine($@");");
            NpgsqlCommand cmdCreateTable = new(strCommand.ToString());
            cmdCreateTable.Parameters.Add(new NpgsqlParameter("@tableName", tableName));
            commands.Add(cmdCreateTable);

            //-+-+-+-+-+-+-+-+
            // Alter Table with Column if Not Exist
            //-+-+-+-+-+-+-+-+
            strCommand.Clear();
            strCommand.AppendLine($@"ALTER TABLE {tableName} ADD COLUMN IF NOT EXISTS {dataName} {sqlDataType[type]};");
            NpgsqlCommand cmdAddCol = new(strCommand.ToString());
            commands.Add(cmdAddCol);

            //-+-+-+-+-+-+-+-+
            // Insert Data
            //-+-+-+-+-+-+-+-+
            strCommand.Clear();
            strCommand.AppendLine($@"INSERT INTO {tableName} (uDex, {dataName})");
            strCommand.AppendLine($@"VALUES (@uDex, @data)");
            strCommand.AppendLine($@"ON CONFLICT (uDex) DO UPDATE SET {dataName} = EXCLUDED.{dataName};");
            NpgsqlCommand cmdAddData = new(strCommand.ToString());
            cmdAddData.Parameters.AddWithValue("@uDex", uDex);
            cmdAddData.Parameters.AddWithValue("@data", data ?? DBNull.Value);
            commands.Add(cmdAddData);

            return commands;
        }

        public const string sqlTableCarts = "carts";
        public const string sqlTableSimps = "simps";
        public const string sqlTableCartItems = "cart_items";

        public static string dropAll()
        {
            StringBuilder sqlCmdText = new();
            sqlCmdText.AppendLine($@"DROP TABLE IF EXISTS {sqlTableCarts} CASCADE;");
            sqlCmdText.AppendLine($@"DROP TABLE IF EXISTS {sqlTableSimps} CASCADE;");
            sqlCmdText.AppendLine($@"DROP TABLE IF EXISTS {sqlTableCartItems} CASCADE;");
            sqlCmdText.AppendLine($@"DROP TYPE IF EXISTS item_type CASCADE;");
            return sqlCmdText.ToString();
        }
    }
}
