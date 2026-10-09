string starterFilePath = Path.Combine(AppContext.BaseDirectory, "items.txt");
ShoppingList list = new ShoppingList(starterFilePath);
ShoppingListMenu menu = new ShoppingListMenu(list);
menu.Run();
