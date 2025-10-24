namespace HoloSimpID {
  [Serializable]
  public struct Item {
    public string Name { get; set; }
    public string Link { get; set; }
    public decimal PriceSGD { get; set; }

    public Item(string name, string link = "", decimal priceSGD = 0m) {
      Name     = name;
      Link     = link;
      PriceSGD = priceSGD;
    }

    public override string ToString() {
      return Name.Hyperlink(Link);
    }
  }
}