# Programming and Object-Oriented Development in C#

## Knowledge Check 2: Robust Shopping List

[Assignment](Assignment.md)

### README Requirements

Your README must contain three things (Bug report, design decision, class diagram)

## 1. Bug Report

Code review before running the program

[Code review](Codereview.md)

Link to Bugreport.md

[Bug report](Bugreport.md)

## 2. Design Decision

`Item` rejects blank names with `ArgumentException` and negative prices with
`ArgumentOutOfRangeException`.

`ShoppingList` has a budget limit of 500 kr. Before adding an item, `Add` checks
whether the current total plus the item's price would exceed this limit. If it
would, `Add` throws a `BudgetExceededException` and does not add the item.
It uses an exception rather than returning `false` because exceeding the budget
is an exceptional failure of the requested operation, and the caller needs the
reason to explain it to the user.

`Program.cs` starts `ShoppingListMenu`, which handles prompts, menu flow, and
user-facing messages. `ShoppingList` owns list operations, budget enforcement,
and file persistence; it reports invalid records as warnings and lets file
errors reach the menu. The menu catches expected exceptions, explains failures
to the user, and keeps the application running. Loading also uses `Add`, so
saved data cannot make the list exceed its budget.

In `ShoppingList.Save`, a `using` block disposes the `StreamWriter` so the file is closed whether saving succeeds or fails. The success message is only shown if writing and closing the file both succeed.

The original `items.txt` is kept as the starter list. Saved changes go to `items.local.txt`, which is ignored by Git. The saved file starts with the budget limit and then stores items as JSON lines, so the limit is restored when the list is loaded. Older JSON-lines and `price;name` files without a budget header can still be loaded and use the default budget. This keeps the starter file unchanged while allowing saved changes to persist between runs on this computer.

## 3. Class Diagram

[View the Mermaid class diagram](Uml.md).

## 4. Running the program

### Prerequisite

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). To
check that it is installed, run:

```bash
dotnet --version
```

### Start the program

Clone the repository, change into its folder, and start the app:

```bash
git clone https://github.com/sofiauhrbom73/mai26ma-kk2-shoppinglist-robust.git
cd mai26ma-kk2-shoppinglist-robust
dotnet run --project Shopping.csproj
```

You can also open the repository folder in Visual Studio Code, open its
integrated terminal, and run the `dotnet run --project Shopping.csproj`
command there.

## 5. How to use the program

When the program starts, it loads the saved list if one exists; otherwise, it
loads the starter items. The menu displays the items, their numbers, the
current total, and the budget limit.

At the `Välj:` prompt, enter one of the menu numbers:

1. **Lägg till vara** - enter a non-empty name and a non-negative whole-number
   price. The item is added only if the total stays within the displayed
   budget. Blank names and invalid prices prompt you to try again. If the item
   would exceed the budget, the program explains why and returns to the menu.
2. **Ta bort vara** - enter the number shown next to the item you want to
   remove. The list is numbered again after each change.
3. **Spara** - save the current list and budget to `items.local.txt` in the
   project folder. Save after making changes that you want to keep.
4. **Sök vara** - enter the item's name to look it up.
5. **Avsluta** - exit the program. Changes made since the last save are not
   saved automatically.

Enter a whole number when asked for a menu choice, item number, or price. The
program explains invalid input and lets you try again. You can also press
Ctrl+C to stop the program.
