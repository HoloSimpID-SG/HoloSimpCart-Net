using System;
using System.Collections.Generic;
using System.Linq;

namespace RuTakingTooLong.src.library
{
    public static partial class MoLibrary
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.Count() == 0;

        public static void AddFrequency<T>(this IDictionary<T, uint> valueFreqMap, T value, uint freq = 1)
        {
            if (valueFreqMap.TryGetValue(value, out uint lastCount))
                valueFreqMap[value] = lastCount + freq;
            else
                valueFreqMap.Add(value, freq);
        }
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            return defaultValue;
        }
        public static TValue GetCastedValueOrDefault<TKey, TValue>(this IDictionary<TKey, object> dictionary, TKey key, Func<object, TValue> castDefinition, TValue defaultValue = default)
        {
            object rawValue = null;
            if (dictionary.TryGetValue(key, out rawValue))
                return castDefinition(rawValue);
            return defaultValue;
        }
        public static string GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, string defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => x as string, defaultValue);
        public static uint GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, uint defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => Convert.ToUInt32(x), defaultValue);
        public static int GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, int defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => Convert.ToInt32(x), defaultValue);
        public static double GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, double defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => Convert.ToDouble(x), defaultValue);
    }
    }