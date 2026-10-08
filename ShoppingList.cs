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

    public IReadOnlyList<Item> Items => items.AsReadOnly();

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
            throw new ArgumentOutOfRangeException(nameof(number), number, "Please enter a valid item number.");
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

    // Writes the user's changes to a local JSON Lines file.
    public void Save()
    {
        using (StreamWriter writer = new StreamWriter(localPath))
        {
            writer.WriteLine($"{BudgetHeaderPrefix}{BudgetLimit}");

            foreach (Item item in items)
            {
                writer.WriteLine(JsonSerializer.Serialize(item));
            }
        }
    }

    // Reads the file back into the list.
    public IReadOnlyList<string> Load()
    {
        List<string> warnings = new List<string>();
        string sourcePath = File.Exists(localPath) ? localPath : starterPath;
        if (!File.Exists(sourcePath))
        {
            return warnings;
        }

        string[] lines = File.ReadAllLines(sourcePath);
        int firstItemLine = 0;

        if (lines.Length > 0 && lines[0].StartsWith(BudgetHeaderPrefix, StringComparison.Ordinal))
        {
            string savedBudget = lines[0][BudgetHeaderPrefix.Length..];
            if (!long.TryParse(savedBudget, out long budgetLimit) || budgetLimit < 0)
            {
                throw new InvalidDataException("Budgetgränsen i filen är ogiltig.");
            }

            BudgetLimit = budgetLimit;
            firstItemLine = 1;
        }

        for (int i = firstItemLine; i < lines.Length; i++)
        {
            try
            {
                if (!TryParseItem(lines[i], out Item item))
                {
                    warnings.Add($"Raden {i + 1} är ogiltig och har hoppats över.");
                    continue;
                }

                Add(item);
            }
            catch (ArgumentException ex)
            {
                warnings.Add($"Ogiltig vara på raden {i + 1}, raden har hoppats över: {ex.Message}");
            }
            catch (BudgetExceededException ex)
            {
                warnings.Add($"Varan på raden {i + 1} har hoppats över: {ex.Message}");
            }
        }

        return warnings;
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
