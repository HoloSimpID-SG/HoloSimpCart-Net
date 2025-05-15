namespace HoloSimpID
{
    public class Simp : IIndexer
    {
        private static uint indexer = 0;
        public uint uDex { get; private set; }

        public string name;
        public Simp(string name)
        {
            uDex = indexer++;

            this.name = name;
        }

        public static implicit operator string(Simp simp) => simp.name;
        public static implicit operator uint(Simp simp) => simp.uDex;
        public static implicit operator int(Simp simp) => (int)simp.uDex;
    }
}
