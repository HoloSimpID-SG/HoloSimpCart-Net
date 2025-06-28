using NpgsqlTypes;

namespace HoloSimpID
{
    [Serializable]
    public struct Item
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public decimal PriceSGD { get; set; }

        public Item(string name, string link = "", decimal priceSGD = 0m)
        {
            this.Name = name;
            this.Link = link;
            this.PriceSGD = priceSGD;
        }

        public override string ToString() => Name.Hyperlink(Link);
    }
}