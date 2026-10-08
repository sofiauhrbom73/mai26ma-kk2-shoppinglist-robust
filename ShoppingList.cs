using System.Text.Json;

// Holds the items and takes care of loading and saving them.
class ShoppingList
{
    private const string BudgetHeaderPrefix = "# ShoppingListBudget=";

    public const long DefaultBudgetLimit = 500;

    public long BudgetLimit { get; private set; }

    private List<Item> items = new List<Item>();
    private readonly string starterPath;
    private readonly string localPath;

    public ShoppingList(string path, long budgetLimit = DefaultBudgetLimit)
    {
        if (budgetLimit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(budgetLimit), budgetLimit, "Budget cannot be negative.");
        }

        BudgetLimit = budgetLimit;
        starterPath = path;
        string directory = Path.GetDirectoryName(path) ?? "";
        string localFileName = $"{Path.GetFileNameWithoutExtension(path)}.local{Path.GetExtension(path)}";
        localPath = Path.Combine(directory, localFileName);
    }

    public void Add(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.Price > BudgetLimit - Total())
        {
            throw new BudgetExceededException(
                $"Adding '{item.Name}' would exceed the {BudgetLimit} kr budget.");
        }

        items.Add(item);
    }

    // Removes the item the user sees as number 1, 2, 3 ...
    public void RemoveAt(int number)
    {
        if (number < 1 || number > items.Count)
        {
            Console.WriteLine("Please enter a valid item number.");
            return;
        }

        items.RemoveAt(number - 1);
    }

    // Adds up the price of every item on the list.
    public long Total()
    {
        long sum = 0;

        for (int i = 0; i < items.Count; i++)
        {
            sum += items[i].Price;
        }

        return sum;
    }

    // Looks up an item by its name. Returns null if there is no such item.
    public Item Find(string name)
    {
        foreach (Item item in items)
        {
            if (item.Name == name)
            {
                return item;
            }
        }

        return null;
    }

    public void Print()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i]}");
        }

        Console.WriteLine($"Totalt: {Total()} kr");
    }

    // Writes the user's changes to a local JSON Lines file.
    public void Save()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(localPath))
            {
                writer.WriteLine($"{BudgetHeaderPrefix}{BudgetLimit}");

                foreach (Item item in items)
                {
                    writer.WriteLine(JsonSerializer.Serialize(item));
                }
            }

            Console.WriteLine("Listan är sparad.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Kunde inte spara listan: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Kunde inte spara listan: {ex.Message}");
        }
    }

    // Reads the file back into the list.
    public void Load()
    {
        string sourcePath = File.Exists(localPath) ? localPath : starterPath;
        if (!File.Exists(sourcePath))
        {
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(sourcePath);
            int firstItemLine = 0;

            if (lines.Length > 0 && lines[0].StartsWith(BudgetHeaderPrefix, StringComparison.Ordinal))
            {
                string savedBudget = lines[0][BudgetHeaderPrefix.Length..];
                if (!long.TryParse(savedBudget, out long budgetLimit) || budgetLimit < 0)
                {
                    Console.WriteLine("Kunde inte läsa inköpslistan: budgetgränsen i filen är ogiltig.");
                    return;
                }

                BudgetLimit = budgetLimit;
                firstItemLine = 1;
            }

            for (int i = firstItemLine; i < lines.Length; i++)
            {
                try
                {
                    if (TryParseItem(lines[i], out Item item))
                    {
                        Add(item);
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Ogiltig vara i filen, raden hoppas över: {ex.Message}");
                }
                catch (BudgetExceededException ex)
                {
                    Console.WriteLine($"Varan i filen hoppas över: {ex.Message}");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Kunde inte läsa inköpslistan: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Kunde inte läsa inköpslistan: {ex.Message}");
        }
    }

    private static bool TryParseItem(string line, out Item item)
    {
        item = null;
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        if (line.TrimStart().StartsWith("{", StringComparison.Ordinal))
        {
            try
            {
                item = JsonSerializer.Deserialize<Item>(line);
                return item != null && !string.IsNullOrWhiteSpace(item.Name);
            }
            catch (JsonException)
            {
                return false;
            }
        }

        int separator = line.IndexOf(';');
        if (separator < 1
            || !int.TryParse(line[..separator], out int price)
            || string.IsNullOrWhiteSpace(line[(separator + 1)..]))
        {
            return false;
        }

        item = new Item(line[(separator + 1)..], price);
        return true;
    }
}
