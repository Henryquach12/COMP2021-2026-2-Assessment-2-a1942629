using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class RecipeManagerTests
{
    [Fact]
    public void ConstructorBuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    [Fact]
    // Test RecipeManager rejects null Recipe list.
    public void ConstructorRejectsNullRecipeList()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    // Test RecipeManager rejects null Recipe in Recipe list.
    public void ConstructorRejectsNullRecipeInList()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(new Recipe[] { null! }));
    }

    [Fact]
    // Test RecipeManager rejects Recipe with non-positive Id.
    public void ConstructorRejectsNonPositiveId()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]{new Recipe{
            Id = -20,
            Title = "Recipe A",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }}));
    }

    [Fact]
    // Test RecipeManager rejects Recipe with blank title.
    public void ConstructorRejectsBlankTitle()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]{new Recipe{
            Id = 20,
            Title = "",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }}));
    }

    [Fact]
    // Test RecipeManager rejects Recipe with duplicate Id.
    public void ConstructorRejectsDuplicateId()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 20,
                Title = "Recipe B",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe C",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            }
        }));
    }

    [Fact]
    // Test AddRecipe successfully adds Recipe.
    public void AddRecipeSuccessfullyAddsRecipe()
    {
        var manager = CreateManager();
        var recipe = CreateRecipe();
        Assert.Equal(2, manager.RecipeCount);

        bool addedRecipe = manager.AddRecipe(recipe);
        Assert.Equal(3, manager.RecipeCount);
        Assert.True(addedRecipe);

        // FindRecipe can find new added Recipe.
        Recipe? added = manager.FindRecipe(30);
        Assert.NotNull(added);
        Assert.Equal(30, added.Id);
    }

    [Fact]
    // Test AddRecipe rejects null Recipe.
    public void AddRecipeRejectsNullRecipe()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    [Fact]
    // Test AddRecipe rejects Recipe with non-positive Id.
    public void AddRecipeRejectsNonPositiveId()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);

        Assert.False(manager.AddRecipe(new Recipe
        {
            Id = -20,
            Title = "Recipe A",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
        }));

        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    // Test AddRecipe rejects Recipe with blank title.
    public void AddRecipeRejectsBlankTitle()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);

        Assert.False(manager.AddRecipe(new Recipe
        {
            Id = 40,
            Title = "",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
        }));

        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    // Test AddRecipe rejects Recipe with duplicate Id.
    public void AddRecipeRejectsDuplicateId()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);

        Assert.False(manager.AddRecipe(new Recipe
        {
            Id = 10,
            Title = "Recipe D",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
        }));

        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    // Test FindRecipe returns matching Recipe.
    public void FindRecipeReturnsMatchingRecipe()
    {
        var manager = CreateManager();

        Recipe? recipe = manager.FindRecipe(10);
        Assert.NotNull(recipe);
        Assert.Equal(10, recipe.Id);
        Assert.Equal("Recipe A", recipe.Title);
    }

    [Fact]
    // Test FindRecipe returns null for missing Recipe Id.
    public void FindRecipeReturnsNullForMissingRecipeId()
    {
        var manager = CreateManager();

        Recipe? recipe = manager.FindRecipe(50);
        Assert.Null(recipe);
    }

    [Fact]
    // Test RemoveRecipe successfully removes Recipe and returns true.
    public void RemoveRecipeSuccessfullyRemovesRecipe()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);

        Assert.True(manager.RemoveRecipe(10));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    // Test RemoveRecipe returns false for missing Recipe Id.
    public void RemoveRecipeReturnsFalseForMissingRecipeId()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);

        Assert.False(manager.RemoveRecipe(100));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    // Test RemoveRecipe returns false when Recipe is in cooking plan.
    public void RemoveRecipeReturnsFalseWhenRecipeIsInCookingPlan()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);

        manager.AddRecipeToCookingPlan(10);

        Assert.False(manager.RemoveRecipe(10));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    // Test AddIngredientsToShoppingList successfully adds ingredients and returns the number of ingredients added.
    public void AddIngredientsToShoppingListSuccessfullyAddsIngredients()
    {
        var manager = CreateManager();
        Assert.Equal(0, manager.ShoppingItemCount);

        // The returned total ingredients matches the exact item in shopping list.
        int numberOfIngredients = manager.AddIngredientsToShoppingList(10);
        Assert.Equal(2, numberOfIngredients);
        Assert.Equal(2, manager.ShoppingItemCount);

        // Find exact added items in shopping list.
        var shoppingList = manager.GetShoppingList();
        Assert.Equal("1 apple", shoppingList[0]);
        Assert.Equal("2 banana", shoppingList[1]);
    }

    [Fact]
    // Test AddIngredientsToShoppingList returns zero for missing Recipe Id.
    public void AddIngredientsToShoppingListReturnsZeroForMissingRecipeId()
    {
        var manager = CreateManager();
        Assert.Equal(0, manager.ShoppingItemCount);

        int numberOfIngredients = manager.AddIngredientsToShoppingList(100);
        Assert.Equal(0, numberOfIngredients);

        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    // Test GetShoppingList returns a copy of the shopping list without exposing internal list.
    public void GetShoppingListReturnsCopyWithoutExposingInternalList()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);

        var result = manager.GetShoppingList();
        Assert.Equal(2, result.Count);
        Assert.Equal("1 apple", result[0]);
        Assert.Equal("2 banana", result[1]);

        // Clearing real shopping list does not affect the copy.
        manager.ClearShoppingList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1 apple", result[0]);
        Assert.Equal("2 banana", result[1]);
    }

    [Fact]
    // Test ClearShoppingList successfully removes all items in the shopping list.
    public void ClearShoppingListSuccessfullyRemovesAllItems()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        manager.ClearShoppingList();

        // Shopping list is empty after clearing.
        var result = manager.GetShoppingList();
        Assert.Empty(result);

        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    // Test AddRecipeToCookingPlan successfully adds Recipe to cooking plan.
    public void AddRecipeToCookingPlanSuccessfullyAddsRecipe()
    {
        var manager = CreateManager();

        var result = manager.AddRecipeToCookingPlan(10);
        Assert.True(result);

        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    // Test AddRecipeToCookingPlan returns false for missing Recipe Id.
    public void AddRecipeToCookingPlanReturnsFalseForMissingRecipeId()
    {
        var manager = CreateManager();

        var result = manager.AddRecipeToCookingPlan(100);
        Assert.False(result);

        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    // Test AddRecipeToCookingPlan rejects duplicate Recipe in cooking plan.
    public void AddRecipeToCookingPlanRejectsDuplicateRecipe()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        var result = manager.AddRecipeToCookingPlan(10);
        Assert.False(result);

        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    // Test RemoveRecipeFromCookingPlan successfully removes recipe from the cooking plan.
    public void RemoveRecipeFromCookingPlanSuccessfullyRemovesRecipe()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        
        var result = manager.RemoveRecipeFromCookingPlan(10);
        Assert.True(result);

        Assert.Equal(0, manager.CookingPlanCount);
        Assert.Equal(1, manager.RemovedRecipeCount);
    }

    [Fact]
    // Test RemoveRecipeFromCookingPlan returns false when Recipe is not in cooking plan.
    public void RemoveRecipeFromCookingPlanReturnsFalseWhenRecipeIsNotInCookingPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        var result = manager.RemoveRecipeFromCookingPlan(20);
        Assert.False(result);

        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    // Test RestoreLastRemovedRecipe successfully restores last removed Recipe.
    public void RestoreLastRemovedRecipeSuccessfullyRestoresRecipe()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(1, manager.RemovedRecipeCount);

        bool result = manager.RestoreLastRemovedRecipe();
        Assert.True(result);

        Assert.Equal(2, manager.CookingPlanCount);
        Assert.Equal(0, manager.RemovedRecipeCount);

        // Find restored Recipe in cooking plan.
        var plan = manager.GetCookingPlan(); 
        Assert.Equal(20, plan.First()); 
        Assert.Equal(10, plan.Last());
    }

    [Fact]
    // Test RestoreLastRemovedRecipe returns false when removed Recipe stack is empty.
    public void RestoreLastRemovedRecipeReturnsFalseWhenStackIsEmpty()
    {
        var manager = CreateManager();
        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);

        bool result = manager.RestoreLastRemovedRecipe();
        Assert.False(result);

        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    // Test RestoreLastRemovedRecipe returns false when the removed Recipe no longer exists.
    public void RestoreLastRemovedRecipeReturnsFalseWhenRecipeNoLongerExists()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);

        // Remove the removed Recipe.
        manager.RemoveRecipe(10);
        Assert.Null(manager.FindRecipe(10));

        bool result = manager.RestoreLastRemovedRecipe();
        Assert.False(result);

        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    // Test RestoreLastRemovedRecipe returns false when the removed Recipe is already in the cooking plan.
    public void RestoreLastRemovedRecipeReturnsFalseWhenRecipeIsAlreadyInCookingPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);
        
        // Add the removed Recipe to the cooking plan.
        manager.AddRecipeToCookingPlan(10);

        bool result = manager.RestoreLastRemovedRecipe();
        Assert.False(result);

        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    // Test PeekLastRemovedRecipe returns null when the removed recipe stack is empty.
    public void PeekLastRemovedRecipeReturnsNullWhenStackIsEmpty()
    {
        var manager = CreateManager();

        Assert.Equal(0, manager.RemovedRecipeCount);

        int? result = manager.PeekLastRemovedRecipe();
        Assert.Null(result);
    }

    [Fact]
    // Test PeekLastRemovedRecipe successfully returns last removed Recipe Id.
    public void PeekLastRemovedRecipeSuccessfullyReturnsLastRemovedRecipeId()
    {
        var manager = CreateManager();
        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.AddRecipeToCookingPlan(20));
        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.True(manager.RemoveRecipeFromCookingPlan(20));

        Assert.Equal(2, manager.RemovedRecipeCount);

        int? result = manager.PeekLastRemovedRecipe();
        Assert.Equal(20, result);

        Assert.Equal(2, manager.RemovedRecipeCount);
    }

    [Fact]
    // Test GetCookingPlan returns a copy of the cooking plan without exposing internal plan.
    public void GetCookingPlanReturnsCopyWithoutExposingInternalPlan()
    {
        var manager = CreateManager();
        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.AddRecipeToCookingPlan(20));

        var result = manager.GetCookingPlan();
        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0]);
        Assert.Equal(20, result[1]);

        // Removing a recipe from the cooking plan does not impact the copy.
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0]);
        Assert.Equal(20, result[1]);
    }

    [Fact]
    // Test GetCookingPlan returns empty list when cooking plan is empty.
    public void GetCookingPlanReturnsEmptyListWhenPlanIsEmpty()
    {
        var manager = CreateManager();

        var result = manager.GetCookingPlan();
        Assert.Empty(result);
    }

    [Fact]
    // Test StartCooking successfully adds Recipe instructions to instruction queue.
    public void StartCookingSuccessfullyAddsInstructionsToInstructionQueue()
    {
        var manager = CreateManager();

        var result = manager.StartCooking(10);
        Assert.True(result);

        Assert.Equal(2, manager.PendingInstructionCount);
    }

    [Fact]
    // Test StartCooking returns false for missing Recipe Id.
    public void StartCookingReturnsFalseForMissingRecipeId()
    {
        var manager = CreateManager();

        var result = manager.StartCooking(30);
        Assert.False(result);
        
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    // Test StartCooking returns false for Recipe with no instructions.
    public void StartCookingReturnsFalseForRecipeWithNoInstructions()
    {
        var manager = CreateManager();

        // Add Recipe that has empty instructions.
        Recipe newRecipe = new Recipe
        {
            Id = 40,
            Title = "Recipe D",
            Ingredients = new() { "2 apple" },
            Instructions = new() { }
        };
        manager.AddRecipe(newRecipe);

        var result = manager.StartCooking(newRecipe.Id);
        Assert.False(result);

        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    // Test StartCooking clears leftover instructions before cooking the new Recipe.
    public void StartCookingClearsLeftoverInstructionsFromPreviousRecipe()
    {
        var manager = CreateManager();
        manager.AddRecipe(CreateRecipe());

        // Init cooking Recipe.
        Assert.True(manager.StartCooking(10));
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.True(manager.StartCooking(30));
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Third step", manager.CompleteNextInstruction());
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    // Test PeekNextInstruction successfully returns the next instruction without removing it from the queue.
    public void PeekNextInstructionSuccessfullyReturnsNextInstruction()
    {
        var manager = CreateManager();
        manager.StartCooking(10);

        string? result = manager.PeekNextInstruction();
        Assert.Equal("First step", result);

        Assert.Equal(2, manager.PendingInstructionCount);
    }

    [Fact]
    // Test PeekNextInstruction returns null when the instruction queue is empty.
    public void PeekNextInstructionReturnsNullWhenQueueIsEmpty()
    {
        var manager = CreateManager();
        Assert.Equal(0, manager.PendingInstructionCount);

        string? result = manager.PeekNextInstruction();
        Assert.Null(result);
    }

    [Fact]
    // Test CompleteNextInstruction successfully removes and returns exactly one instruction from the front of the queue.
    public void CompleteNextInstructionSuccessfullyRemovesAndReturnsFirstInstruction()
    {
        var manager = CreateManager();
        manager.StartCooking(10);

        string? result = manager.CompleteNextInstruction();
        Assert.Equal("First step", result);

        Assert.Equal(1, manager.PendingInstructionCount);

        // Peek find second instruction instead of first instruction.
        string? nextResult = manager.PeekNextInstruction();
        Assert.Equal("Second step", nextResult);

        Assert.Equal(1, manager.PendingInstructionCount);
    }

    [Fact]
    // Test CompleteNextInstruction returns null when the instruction queue is empty.
    public void CompleteNextInstructionReturnsNullWhenQueueIsEmpty()
    {
        var manager = CreateManager();
        Assert.Equal(0, manager.PendingInstructionCount);

        string? result = manager.CompleteNextInstruction();
        Assert.Null(result);
    }

    [Fact]
    // Test SearchByTitle successfully returns every recipe whose title contains the search text.
    public void SearchByTitleSuccessfullyReturnsRecipeListMatchInputTitle()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.SearchByTitle("Chicken");

        // "Chicken Curry" and "chicken salad" are differed in case but they both match.
        Assert.Equal(2, result.Count);
        Assert.Contains(result, recipe => recipe.Id == 1);
        Assert.Contains(result, recipe => recipe.Id == 2);
    }

    [Fact]
    // Test SearchByTitle returns no Recipe when the search text is blank.
    public void SearchByTitleReturnsEmptyForBlankSearchText()
    {
        var manager = CreateSearchManager();

        // Empty and white-space search text are both treated as blank.
        Assert.Empty(manager.SearchByTitle(""));
        Assert.Empty(manager.SearchByTitle("   "));
    }

    [Fact]
    // Test SearchByTitle returns no Recipe when no title matches the search text.
    public void SearchByTitleReturnsEmptyForMissingTitle()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.SearchByTitle("pizza");

        Assert.Empty(result);
    }

    [Fact]
    // Test SearchByIngredient successfully returns every recipe that contains the search text in its ingredients.
    public void SearchByIngredientSuccessfullyReturnsRecipeListMatchInputIngredient()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.SearchByIngredient("onion");

        // "1 onion" appears in three recipes so all three match.
        Assert.Equal(3, result.Count);
        Assert.Contains(result, recipe => recipe.Id == 1);
        Assert.Contains(result, recipe => recipe.Id == 3);
        Assert.Contains(result, recipe => recipe.Id == 4);
    }

    [Fact]
    // Test SearchByIngredient returns a Recipe only once when several of its ingredients match.
    public void SearchByIngredientReturnsEachRecipeOnlyOnce()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.SearchByIngredient("chicken");

        // Even recipe 1 contains both "500g chicken" and "1 tsp chicken stock" ingredients but it is appeared once in the list.
        Assert.Equal(2, result.Count);
        Assert.Single(result, recipe => recipe.Id == 1);
    }

    [Fact]
    // Test SearchByIngredient returns no Recipe when the search text is blank.
    public void SearchByIngredientReturnsEmptyForBlankSearchText()
    {
        var manager = CreateSearchManager();

        // Empty and white-space search text are both treated as blank.
        Assert.Empty(manager.SearchByIngredient(""));
        Assert.Empty(manager.SearchByIngredient("   "));
    }

    [Fact]
    // Test SearchByIngredient returns no Recipe when no ingredient matches the search text.
    public void SearchByIngredientReturnsEmptyForMissingIngredient()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.SearchByIngredient("watermelon");

        Assert.Empty(result);
    }

    [Fact]
    // Test GetHighestProteinRecipes successfully returns up to the requested number of Recipes ordered by highest available protein value.
    public void GetHighestProteinRecipesSuccessfullyReturnsCountRecipesOrderedByProteinDescending()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.GetHighestProteinRecipes(2);

        Assert.Equal(2, result.Count);
        Assert.Equal(3, result[0].Id);
        Assert.Equal(1, result[1].Id);
    }

    [Fact]
    // Test GetHighestProteinRecipes returns no Recipe with null ProteinG.
    public void GetHighestProteinRecipesReturnsEmptyForNullProteinG()
    {
        var manager = new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 1,
                Title = "Fake cake",
                Nutrition = new NutritionInfo { ProteinG = null }
            }
        });

        IReadOnlyList<Recipe> result = manager.GetHighestProteinRecipes(5);

        Assert.Empty(result);
    }

    [Fact]
    // Test GetHighestProteinRecipes returns no Recipe when count is zero.
    public void GetHighestProteinRecipesReturnsEmptyForZeroCount()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.GetHighestProteinRecipes(0);

        Assert.Empty(result);
    }

    [Fact]
    // Test GetHighestProteinRecipes returns no Recipe when count is negative.
    public void GetHighestProteinRecipesReturnsEmptyForNegativeCount()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.GetHighestProteinRecipes(-1);

        Assert.Empty(result);
    }

    [Fact]
    // Test GetHighestProteinRecipes excludes Recipe with null Nutrition.
    public void GetHighestProteinRecipesExcludesNullNutrition()
    {
        var manager = CreateSearchManager();

        IReadOnlyList<Recipe> result = manager.GetHighestProteinRecipes(5);

        Assert.Equal(3, result.Count);
        Assert.DoesNotContain(result, recipe => recipe.Id == 4);
    }

    [Fact]
    // Test AddSavedRecipe successfully adds an existing Recipe Id.
    public void AddSavedRecipeSuccessfullyAddsExistingRecipe()
    {
        var manager = CreateSearchManager();

        bool result = manager.AddSavedRecipe(1);

        Assert.True(result);
    }

    [Fact]
    // Test AddSavedRecipe returns false for a Recipe Id that does not exist.
    public void AddSavedRecipeReturnsFalseForMissingRecipe()
    {
        var manager = CreateSearchManager();

        bool result = manager.AddSavedRecipe(99);

        Assert.False(result);
    }

    [Fact]
    // Test AddSavedRecipe returns false when the Recipe Id is already saved.
    public void AddSavedRecipeReturnsFalseForDuplicateRecipe()
    {
        var manager = CreateSearchManager();        
        manager.AddSavedRecipe(1);

        bool result = manager.AddSavedRecipe(1);

        Assert.False(result);
    }

    [Fact]
    // Test RemoveSavedRecipe successfully removes a saved Recipe Id.
    public void RemoveSavedRecipeSuccessfullyRemovesSavedRecipe()
    {
        var manager = CreateSearchManager();
        manager.AddSavedRecipe(1);

        bool result = manager.RemoveSavedRecipe(1);

        Assert.True(result);
    }

    [Fact]
    // Test RemoveSavedRecipe returns false when the Recipe Id is not saved.
    public void RemoveSavedRecipeReturnsFalseForUnsavedRecipe()
    {
        var manager = CreateSearchManager();

        bool result = manager.RemoveSavedRecipe(1);

        Assert.False(result);
    }

    [Fact]
    // Test RemoveSavedRecipe returns false when the Recipe Id is removed twice.
    public void RemoveSavedRecipeReturnsFalseWhenRemovedTwice()
    {
        var manager = CreateSearchManager();
        manager.AddSavedRecipe(1);
        manager.RemoveSavedRecipe(1);

        bool result = manager.RemoveSavedRecipe(1);

        Assert.False(result);
    }

    [Fact]
    // Test IsRecipeSaved returns true when the Recipe Id is saved.
    public void IsRecipeSavedSuccessfullyReturnsTrueForSavedRecipe()
    {
        var manager = CreateSearchManager();
        manager.AddSavedRecipe(1);

        bool result = manager.IsRecipeSaved(1);

        Assert.True(result);
    }

    [Fact]
    // Test IsRecipeSaved returns false when the Recipe Id is not saved.
    public void IsRecipeSavedReturnsFalseForUnsavedRecipe()
    {
        var manager = CreateSearchManager();

        bool result = manager.IsRecipeSaved(1);

        Assert.False(result);
    }

    [Fact]
    // Test IsRecipeSaved returns false after the Recipe Id is removed from saved recipes.
    public void IsRecipeSavedReturnsFalseAfterRecipeRemoved()
    {
        var manager = CreateSearchManager();
        manager.AddSavedRecipe(1);
        manager.RemoveSavedRecipe(1);

        bool result = manager.IsRecipeSaved(1);

        Assert.False(result);
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple", "2 banana" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }

    private static Recipe CreateRecipe()
    {
        return new Recipe
        {
            Id = 30,
            Title = "Recipe C",
            Ingredients = new() { "2 apple" },
            Instructions = new() { "First step", "Third step" }
        };
    }

    private static RecipeManager CreateSearchManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 1,
                Title = "Chicken Curry",
                Ingredients = new() { "500g chicken", "1 tsp chicken stock", "1 onion" },
                Instructions = new() { "Fry the onion", "Add the chicken" },
                Nutrition = new NutritionInfo { ProteinG = 32.5 }
            },
            new Recipe
            {
                Id = 2,
                Title = "chicken salad",
                Ingredients = new() { "200g chicken", "1 lettuce", "2 tomato" },
                Instructions = new() { "Chop the lettuce" },
                Nutrition = new NutritionInfo { ProteinG = 18.0 }
            },
            new Recipe
            {
                Id = 3,
                Title = "Beef Stew",
                Ingredients = new() { "600g beef", "3 carrot", "1 onion" },
                Instructions = new() { "Brown the beef" },
                Nutrition = new NutritionInfo { ProteinG = 41.2 }
            },
            new Recipe
            {
                Id = 4,
                Title = "Tomato Soup",
                Ingredients = new() { "6 tomato", "1 onion" },
                Instructions = new() { "Simmer the tomato" },
                Nutrition = null
            },
        });
    }
}
