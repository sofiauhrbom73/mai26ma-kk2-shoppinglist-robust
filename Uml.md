# Shopping List Class Diagram

The diagram shows the program's main classes, their responsibilities, and how
they relate to one another.

```mermaid
classDiagram
    class Program {
        <<top-level entry point>>
    }

    class ShoppingListMenu {
        +Run()
        -AddItem()
        -RemoveItem()
        -SaveList()
        -LoadList()
        -FindItem()
        -ReadInteger() int?
        -ReadName() string
    }

    class ShoppingList {
        +DefaultBudgetLimit long
        +BudgetLimit long
        +Items IReadOnlyList~Item~
        -items List~Item~
        -starterPath string
        -localPath string
        +ShoppingList(path, budgetLimit)
        +Add(item)
        +RemoveAt(number)
        +Total() long
        +Find(name) Item
        +Save()
        +Load() IReadOnlyList~string~
    }

    class Item {
        +Name string
        +Price int
        +Item(name, price)
        +ToString() string
    }

    class BudgetExceededException {
        +BudgetExceededException(message)
    }

    class Exception

    Program ..> ShoppingListMenu : starts
    ShoppingListMenu ..> ShoppingList : uses
    ShoppingListMenu ..> Item : creates
    ShoppingListMenu ..> BudgetExceededException : catches
    ShoppingList "1" *-- "0..*" Item : contains
    ShoppingList ..> BudgetExceededException : throws
    BudgetExceededException --|> Exception : inherits
```

`ShoppingList.Save()` writes the budget header and item data to the local file.
`ShoppingList.Load()` restores the saved budget and items. `Item` rejects blank
names and negative prices, while `ShoppingList` rejects additions that exceed
its budget.
