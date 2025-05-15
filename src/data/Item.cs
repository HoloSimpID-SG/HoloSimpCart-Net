namespace HoloSimpID
{
    public class Item 
    {
        public string name;
        public string link;
        public double priceSGD;

        public Item(string name, string link = "", double priceSGD = 0)
        {
            this.name = name;
            this.link = link;
            this.priceSGD = priceSGD;
        }

        public static implicit operator string(Item item) => item.name;
    }
}
