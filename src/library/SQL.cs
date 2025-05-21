using Microsoft.Data.SqlClient;
using System.Collections.Immutable;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace HoloSimpID
{
    public static partial class MoLibrary
    {
        public static string ToSqlString(this string str) => '\'' + str.Replace("'", "''") + '\'';
        public static string ToSqlDate(this DateTime dateTime) => '\'' + $"{dateTime:o}" + '\'';

        public static T GetCastedValueOrDefault<T>(this IDataReader reader, string key, Func<object, T> castDefinition, T defaultValue = default)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == key)
                    return castDefinition(reader[i]);
            }
            return defaultValue;
        }
        public static string GetCastedValueOrDefault(this IDataReader reader, string key, string defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => x as string, defaultValue);
        public static DateTime GetCastedValueOrDefault(this IDataReader reader, string key, DateTime defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => DateTime.Parse(x as string, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind), defaultValue);
        public static uint GetCastedValueOrDefault(this IDataReader reader, string key, uint defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => Convert.ToUInt32(x), defaultValue);
        public static int GetCastedValueOrDefault(this IDataReader reader, string key, int defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => Convert.ToInt32(x), defaultValue);
        public static double GetCastedValueOrDefault(this IDataReader reader, string key, double defaultValue = default) =>
            GetCastedValueOrDefault(reader, key, x => Convert.ToDouble(x), defaultValue);

        public static readonly ImmutableDictionary<Type, string> sqlDataType = new Dictionary<Type, string>()
        {
            { typeof(string), "TEXT" },
            { typeof(uint), "INT" },
            { typeof(int), "INT" },
            { typeof(double), "DOUBLE PRECISION" },
        }.ToImmutableDictionary();
        public const string sqlIndex = "uDex";
        public static List<SqlCommand> SafeUpsert(
            string tableName, uint uDex,
            object data, [CallerArgumentExpression("property")] string dataName = null)
        {
            List<SqlCommand> commands = new();
            StringBuilder strCommand = new();
            if (data is DateTime dateTime)
                data = $"{dateTime:o}";
            Type type = data.GetType();

            //-+-+-+-+-+-+-+-+
            // Create Table if Not Exist
            //-+-+-+-+-+-+-+-+
            strCommand.Clear();
            strCommand.AppendLine($@"IF NOT EXISTS (");
            strCommand.AppendLine($@"   SELECT 1 FROM INFORMATION_SCHEMA.TABLES");
            strCommand.AppendLine($@"   WHERE TABLE_NAME = @tableName AND TABLE_SCHEMA = 'dbo'");
            strCommand.AppendLine($@")");
            strCommand.AppendLine($@"BEGIN");
            strCommand.AppendLine($@"    CREATE TABLE [{tableName}] (");
            strCommand.AppendLine($@"    [{sqlIndex}] INT PRIMARY KEY");
            strCommand.AppendLine($@")");
            strCommand.AppendLine($@"END");
            SqlCommand cmdCreateTable = new(strCommand.ToString());
            cmdCreateTable.Parameters.Add(new SqlParameter("@tableName", tableName));
            commands.Add(cmdCreateTable);

            //-+-+-+-+-+-+-+-+
            // Alter Table with Column if Not Exist
            //-+-+-+-+-+-+-+-+
            strCommand.Clear();
            strCommand.AppendLine($@"IF NOT EXISTS (");
            strCommand.AppendLine($@"    SELECT 1");
            strCommand.AppendLine($@"    FROM INFORMATION_SCHEMA.COLUMNS");
            strCommand.AppendLine($@"    WHERE TABLE_NAME = @tableName");
            strCommand.AppendLine($@"       AND COLUMN_NAME = @columnName");
            strCommand.AppendLine($@"BEGIN");
            strCommand.AppendLine($@"   ALTER TABLE [{tableName}] ADD [{dataName}] {sqlDataType[type]};");
            strCommand.AppendLine($@"END");
            SqlCommand cmdAddCol = new(strCommand.ToString());
            cmdAddCol.Parameters.Add(new SqlParameter("@tableName", tableName));
            cmdAddCol.Parameters.Add(new SqlParameter("@columnName", dataName));
            commands.Add(cmdAddCol);

            //-+-+-+-+-+-+-+-+
            // Insert Data
            //-+-+-+-+-+-+-+-+
            strCommand.Clear();
            strCommand.AppendLine($@"IF EXISTS (SELECT 1 FROM [{tableName}] WHERE [{sqlIndex}] = @uDex)");
            strCommand.AppendLine($@"   UPDATE [{tableName}] SET [{dataName}] = @data WHERE [{sqlIndex}] = @uDex");
            strCommand.AppendLine($@"ELSE");
            strCommand.AppendLine($@"   INSERT INTO [{tableName}] ([{sqlIndex}], [{dataName}]) VALUES (@uDex, @data)");
            SqlCommand cmdAddData = new(strCommand.ToString());
            cmdAddData.Parameters.Add(new SqlParameter("@uDex", uDex));
            cmdAddData.Parameters.Add(new SqlParameter("@data", data ?? DBNull.Value));
            commands.Add(cmdAddData);

            return commands;
        }
    }
}
