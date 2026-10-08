class ShoppingListMenu
{
    private readonly ShoppingList list;

    public ShoppingListMenu(ShoppingList list)
    {
        this.list = list ?? throw new ArgumentNullException(nameof(list));
    }

    public void Run()
    {
        LoadList();

        while (true)
        {
            Console.WriteLine();
            PrintList();
            Console.WriteLine();
            Console.WriteLine("1. Lägg till vara");
            Console.WriteLine("2. Ta bort vara");
            Console.WriteLine("3. Spara");
            Console.WriteLine("4. Sök vara");
            Console.WriteLine("5. Avsluta");

            int? choice = ReadInteger("Välj: ", 1, 5);
            if (choice == null || choice == 5)
            {
                return;
            }

            bool continueRunning;
            switch (choice.Value)
            {
                case 1:
                    continueRunning = AddItem();
                    break;
                case 2:
                    continueRunning = RemoveItem();
                    break;
                case 3:
                    SaveList();
                    continueRunning = true;
                    break;
                case 4:
                    continueRunning = FindItem();
                    break;
                default:
                    continueRunning = true;
                    break;
            }

            if (!continueRunning)
            {
                return;
            }
        }
    }

    private void LoadList()
    {
        try
        {
            foreach (string warning in list.Load())
            {
                Console.WriteLine(warning);
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

    private void PrintList()
    {
        for (int i = 0; i < list.Items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {list.Items[i]}");
        }

        Console.WriteLine($"Totalt: {list.Total()} kr av {list.BudgetLimit} kr");
    }

    private bool AddItem()
    {
        string name = ReadName("Namn: ");
        if (name == null)
        {
            return false;
        }

        int? price = ReadInteger("Pris: ", 0, int.MaxValue);
        if (price == null)
        {
            return false;
        }

        try
        {
            list.Add(new Item(name, price.Value));
            Console.WriteLine("Varan har lagts till.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ogiltig vara: {ex.Message}");
        }
        catch (BudgetExceededException ex)
        {
            Console.WriteLine(ex.Message);
        }

        return true;
    }

    private bool RemoveItem()
    {
        int? number = ReadInteger("Nummer: ", 1, int.MaxValue);
        if (number == null)
        {
            return false;
        }

        try
        {
            list.RemoveAt(number.Value);
            Console.WriteLine("Varan har tagits bort.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine(ex.Message);
        }

        return true;
    }

    private void SaveList()
    {
        try
        {
            list.Save();
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

    private bool FindItem()
    {
        string wanted = ReadName("Namn att söka efter: ");
        if (wanted == null)
        {
            return false;
        }

        Item found = list.Find(wanted);
        if (found == null)
        {
            Console.WriteLine("Varan finns inte i listan.");
        }
        else
        {
            Console.WriteLine($"Hittade: {found}");
        }

        return true;
    }

    private static int? ReadInteger(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (input == null)
            {
                return null;
            }

            if (int.TryParse(input, out int value) && value >= minimum && value <= maximum)
            {
                return value;
            }

            Console.WriteLine($"Ange ett heltal mellan {minimum} och {maximum}.");
        }
    }

    private static string ReadName(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string name = Console.ReadLine();
            if (name == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                return name.Trim();
            }

            Console.WriteLine("Namnet får inte vara tomt.");
        }
    }
}
