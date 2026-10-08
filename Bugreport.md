### 1. Bug Report

Describe the six bugs:

* What happened?
* Why did it happen?
* How did you fix it?

A few lines per bug are sufficient.

## Bug 1

* What happened?

The program crashed with an IndexOutOfRangeException when loading the shopping list from the file.

* Why did it happen?

The Load() method assumed that every line contained two values separated by a semicolon. An empty line produced an array with only one element, so accessing parts[1] caused an IndexOutOfRangeException.

The most likely cause of the IndexOutOfRangeException is that parts[1] is accessed without first verifying that the array contains at least two elements.

* How did you fix it?

I added validation to skip empty lines and verify that each line contains exactly two fields before accessing parts[1]. I also replaced int.Parse() with int.TryParse() to handle invalid numeric values safely.