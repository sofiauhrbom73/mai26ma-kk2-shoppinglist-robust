# Programming and Object-Oriented Development in C#

## Knowledge Check 2: Robust Shopping List

[Assignment](Assignment.md)

### README Requirements

Your README must contain three things:

## 1. Bug Report

Link to Bugreport.md

[Bug report](Bugreport.md)

## 2. Design Decision

`ShoppingList` has a budget limit of 500 kr. Before adding an item, `Add` checks whether the current total plus the item's price would exceed this limit. If it would, `Add` throws a `BudgetExceededException` and does not add the item.

I chose an exception because exceeding the budget means the requested operation cannot be completed. The custom exception makes this situation clear to the caller. `ShoppingList.Run` catches it, displays the message to the user, and continues running. The same `try` block also handles invalid item values rejected by the `Item` constructor.

In `ShoppingList.Save`, a `using` block disposes the `StreamWriter` so the file is closed whether saving succeeds or fails. The success message is only shown if writing and closing the file both succeed.

The original `items.txt` is kept as the starter list. Saved changes go to `items.local.txt`, which is ignored by Git. New saves use JSON lines so names containing special characters are preserved; older `price;name` files can still be loaded. This keeps the starter file unchanged while allowing saved changes to persist between runs on this computer.

## 3. Class Diagram

[View the Mermaid class diagram](Uml.md).