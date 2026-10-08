ShoppingList list = new ShoppingList("items.txt");
list.Load();

while (true)
{
    Console.WriteLine();
    list.Print();
    Console.WriteLine();
    Console.WriteLine("1. Lägg till vara");
    Console.WriteLine("2. Ta bort vara");
    Console.WriteLine("3. Spara");
    Console.WriteLine("4. Sök vara");
    Console.WriteLine("5. Avsluta");

    int? choiceInput = ReadInteger("Välj: ", 1, 5);
    if (choiceInput == null)
    {
        break;
    }

    int choice = choiceInput.Value;

    if (choice == 1)
    {
        string name = ReadName("Namn: ");
        if (name == null)
        {
            break;
        }

        int? priceInput = ReadInteger("Pris: ", 0, int.MaxValue);
        if (priceInput == null)
        {
            break;
        }

        int price = priceInput.Value;
        try
        {
            list.Add(new Item(name, price));
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
    }
    else if (choice == 2)
    {
        int? number = ReadInteger("Nummer: ", 1, int.MaxValue);
        if (number == null)
        {
            break;
        }

        list.RemoveAt(number.Value);
    }
    else if (choice == 3)
    {
        list.Save();
    }
    else if (choice == 4)
    {
        string wanted = ReadName("Namn att söka efter: ");
        if (wanted == null)
        {
            break;
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
    }
    else if (choice == 5)
    {
        break;
    }
}

static int? ReadInteger(string prompt, int minimum, int maximum)
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

static string ReadName(string prompt)
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
