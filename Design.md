# Design

## 1. Saved recipe collection

I used HashSet to implement saved/favourite features.

- HashSet is suitable for adding because it prevents duplicate values, and its Add() method also returns a bool based on whether the value was successfully added. This matches the specification requirement for AddSavedRecipe to return a bool, making it easy to determine whether a recipe ID has already been saved.

- HashSet is suitable for removing saved recipes because its Remove() method has an average time complexity of O(1). This is because HashSet uses the hash of the recipe ID to directly find and remove the value, while removing an item from a List requires O(n) time because it may need to loop through each recipe ID to find a match. Therefore, HashSet is more efficient for this operation. Moreover, Remove() in HashSet also returns a bool value, matching the specification for RemoveSavedRecipe.

- HashSet is suitable for checking whether a recipe is saved because its Contains() method has an average time complexity of O(1) since HashSet uses the hash of the recipe ID to find the value directly without needing to check every other value. In comparison, Contains() of List requires O(n) time because it may need to loop through each recipe ID to find a match. This makes HashSet more efficient for membership checks. In addition, Contains() returns a bool, which directly matches the specification requirement for IsRecipeSaved.

## 2. Integration

Part B is built on top of the existing Part A RecipeManager class instead of being implemented as a separate system. The Part B methods operate on the recipe catalogue that Part A already constructs, which is the _recipes dictionary. The title search, ingredient search and protein report use LINQ to query the values of the _recipes dictionary directly, which avoids creating extra copies of the recipe data. Because these queries read from the same catalogue, any recipe added or removed through the Part A AddRecipe or RemoveRecipe methods is immediately showed in search and report results. The saved recipe feature is also dependent on the catalogue. AddSavedRecipe calls the Part A FindRecipe method to verify that a recipe ID exists before it is saved. In addition, the saved collection stores only recipe IDs rather than complete Recipe objects, so recipe details are maintained in a single location and can be retrived from the dictionary when required.

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | Your chosen collection | | Explain how your collection performs membership checks. |
