using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Globalization;

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
    }
}
