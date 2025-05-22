namespace HoloSimpID
{
    public static partial class MoLibrary
    {
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Checks if <paramref name="enumerable"/> <see langword="null"/> or Empty
        /// <br/> - Works for Collection types like <see cref="List{T}"/>,
        /// <br/> - ..<see cref="Dictionary{TKey, TValue}"/> or even <see langword="string"/>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.Count() == 0;

        //-+-+-+-+-+-+-+
        // Freuency Map
        //-+-+-+-+-+-+-+
        #region Frequency Map
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Adds <see cref="Dictionary{TKey, TValue}.Values"/> of <paramref name="valueFreqMap"/> by <paramref name="freq"/>, if it has <paramref name="value"/> as <see cref="Dictionary{TKey, TValue}.Keys"/>.
        /// <br/> - Else add <paramref name="value"/> to <paramref name="valueFreqMap"/> with <paramref name="freq"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static void AddFrequency<T>(this IDictionary<T, uint> valueFreqMap, T value, uint freq = 1) where T : notnull
        {
            if (valueFreqMap.TryGetValue(value, out uint lastCount))
                valueFreqMap[value] = lastCount + freq;
            else
                valueFreqMap.Add(value, freq);
        }
        /// <summary>
        /// <inheritdoc cref="AddFrequency{T}(IDictionary{T, uint}, T, uint)"/>
        /// </summary>
        public static void AddFrequency<T>(this IDictionary<T, uint> valueFreqMap, KeyValuePair<T, uint> valueFreqPair) where T : notnull
        {
            if (valueFreqMap.TryGetValue(valueFreqPair.Key, out uint lastCount))
                valueFreqMap[valueFreqPair.Key] = lastCount + valueFreqPair.Value;
            else
                valueFreqMap.Add(valueFreqPair);
        }
        /// <summary>
        /// <inheritdoc cref="AddFrequency{T}(IDictionary{T, uint}, T, uint)"/>
        /// </summary>
        public static void AddFrequency<T>(this IDictionary<T, uint> valueFreqMap, IEnumerable<T> values) where T : notnull
        {
            foreach (T value in values)
                valueFreqMap.AddFrequency(value);
        }
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Converts a Frequency Map to a <see cref="List{T}"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static List<T> toList<T>(this IDictionary<T, uint> valueFreqMap) where T : notnull
        {
            List<T> list = new();
            foreach(var kvp in valueFreqMap)
                list.AddRange(Enumerable.Repeat(kvp.Key, (int)kvp.Value));
            return list;
        }
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Converts an <see cref="IEnumerable{T}{T}"/> to a Frequency Map.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static void toFrequencyMap<T>(Dictionary<T, uint> valueFreqMap, IEnumerable<T> list) where T : notnull
        {
            foreach (T value in list)
                valueFreqMap.AddFrequency(value);
        }
        /// <summary>
        /// <inheritdoc cref="toFrequencyMap{T}(Dictionary{T, uint}, IEnumerable{T})"/>
        /// </summary>
        public static Dictionary<T, uint> toFrequencyMap<T>(this IEnumerable<T> list) where T : notnull
        {
            Dictionary<T, uint> valueFreqMap = new();
            toFrequencyMap(valueFreqMap, list);
            return valueFreqMap;
        }
        #endregion
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Returns the value of <paramref name="key"/> in <paramref name="dictionary"/> if exists.
        /// <br/> - Returns <paramref name="defaultValue"/> otherwise.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            return defaultValue;
        }
        
        //-+-+-+-+-+-+-+
        // Get Casted Value or Default
        //-+-+-+-+-+-+-+
        #region Get Casted Value or Default
        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Acts like a normal <see cref="Dictionary{TKey, TValue}.TryGetValue"/>.
        /// <br/> - Equipped with a cast, for better comtibility with <see cref="object"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static TValue GetCastedValueOrDefault<TKey, TValue>(this IDictionary<TKey, object> dictionary, TKey key, Func<object, TValue> castDefinition, TValue defaultValue = default)
        {
            object rawValue = null;
            if (dictionary.TryGetValue(key, out rawValue))
                return castDefinition(rawValue);
            return defaultValue;
        }
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{TKey, TValue}(IDictionary{TKey, object}, TKey, Func{object, TValue}, TValue)"/>
        /// </summary>
        public static string GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, string defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => x as string, defaultValue);
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{TKey, TValue}(IDictionary{TKey, object}, TKey, Func{object, TValue}, TValue)"/>
        /// </summary>
        public static uint GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, uint defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => Convert.ToUInt32(x), defaultValue);
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{TKey, TValue}(IDictionary{TKey, object}, TKey, Func{object, TValue}, TValue)"/>
        /// </summary>
        public static int GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, int defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => Convert.ToInt32(x), defaultValue);
        /// <summary>
        /// <inheritdoc cref="GetCastedValueOrDefault{TKey, TValue}(IDictionary{TKey, object}, TKey, Func{object, TValue}, TValue)"/>
        /// </summary>
        public static double GetCastedValueOrDefault<TKey>(this IDictionary<TKey, object> dictionary, TKey key, double defaultValue = default) =>
            GetCastedValueOrDefault(dictionary, key, x => Convert.ToDouble(x), defaultValue);
        #endregion

        /// <summary>
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// <br/> - Compability support of <see cref="List{T}.AddRange(IEnumerable{T})"/> for <see cref="IList{T}"/>.
        /// <br/> -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        /// </summary>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            foreach(T data in range)
                list.Add(data);
        }
    }
}