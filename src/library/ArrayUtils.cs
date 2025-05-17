using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuTakingTooLong.src.library
{
    public static class MoLibrary
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.Count() == 0;

        public static void AddFrequency<T>(this IDictionary<T, uint> valueFreqMap, T value, uint freq = 1)
        {
            if (valueFreqMap.TryGetValue(value, out uint lastCount))
                valueFreqMap[value] = lastCount + freq;
            else
                valueFreqMap.Add(value, freq);
        }
    }
}
