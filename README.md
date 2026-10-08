# Programming and Object-Oriented Development in C#

## Knowledge Check 2: Robust Shopping List

[Assignment](Assignment.md)

### README Requirements

Your README must contain three things:

## 1. Bug Report

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