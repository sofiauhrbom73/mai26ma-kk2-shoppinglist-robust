// One item on the shopping list.
class Item
{
    public string Name { get; }
    public int Price { get; }

    public Item(string name, int price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Item name cannot be empty.", nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), price, "Item price cannot be negative.");
        }

        Name = name;
        Price = price;
    }

    public override string ToString()
    {
        return $"{Name} - {Price} kr";
    }
}
