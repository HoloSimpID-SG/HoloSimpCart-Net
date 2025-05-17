using RuTakingTooLong.src.library;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static System.Windows.Forms.LinkLabel;

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
        //-+-+-+-+-+-+-+-+
        #endregion

        public string name;
        public Dictionary<double, uint> merchSpending;
        public Dictionary<double, uint> miscSpending;
        public Simp(string name)
        {
            //-+-+-+-+-+-+-+-+
            // Indexer
            //-+-+-+-+-+-+-+-+
            #region Indexer
            UDex = indexer++;
            uDexSimps.Add(uDex, this);
            //-+-+-+-+-+-+-+-+
            #endregion

            this.name = name;
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
        public static Simp GetSimp(string cartName)
        {
            var simps = uDexSimps.Where(x => x.Value.name == cartName).Select(x => x.Value);
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
        
        public void addItemToHistory(Item item) => merchSpending.AddFrequency(item.priceSGD);
        public void addItemToHistory(KeyValuePair<Item, uint> itemQuantityPair) => merchSpending.AddFrequency(itemQuantityPair.Key.priceSGD, itemQuantityPair.Value);
        public void addMiscSpending(double priceInSGD) => miscSpending.AddFrequency(priceInSGD);

        public override string ToString() => name;
    }
}
