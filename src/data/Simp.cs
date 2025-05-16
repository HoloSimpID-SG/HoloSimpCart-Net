using RuTakingTooLong.src.library;
using System.Collections.Generic;
using System.Linq;

namespace HoloSimpID
{
    public class Simp : IIndexer
    {
        //-+-+-+-+-+-+-+-+
        // Indexer
        //-+-+-+-+-+-+-+-+
        private static uint indexer = 0;
        public uint uDex => UDex; private readonly uint UDex;

        private static readonly Dictionary<uint, Simp> uDexSimps = new();
        //-+-+-+-+-+-+-+-+

        public string name;
        public Simp(string name)
        {
            UDex = indexer++;
            uDexSimps.Add(uDex, this);

            this.name = name;
        }
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
        /// <br/> - Returns the first <see cref="Cart"/> with <see cref="Cart.cartName"/> that matches the <paramref name="cartName"/>.
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

        public static implicit operator string(Simp simp) => simp.name;
        public static implicit operator uint(Simp simp) => simp.uDex;
        public static implicit operator int(Simp simp) => (int)simp.uDex;
    }
}
