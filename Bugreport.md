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

## Bug 2

* What happened?

After fixing the first bug, I see that the total amount is wrong.

* Why did it happen?

The loop starts at index 1 instead of 0, causing the first item in the list to be excluded from the total calculation.

for (int i = 1; i < items.Count; i++)

This is likely the defect referred to in the assignment as "produces an incorrect result without causing the program to crash."

public int Total()
    {
        int sum = 0;

        for (int i = 1; i < items.Count; i++)
        {
            sum += items[i].Price;
        }

        return sum;
    }

* How did you fix it?

Start to count from 0.

for (int i = 0; i < items.Count; i++)