using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
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
    // Test RecipeManager constructor successfully builds Recipe dictionary.
    public void Constructor_SuccessfullyBuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    // Test RecipeManager rejects null Recipe list.
    public void Constructor_RejectsNullRecipeList()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    // Test RecipeManager rejects null Recipe in Recipe list.
    public void Constructor_RejectsNullRecipeInList()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(new Recipe[] { null! }));
    }

    [Fact]
    // Test RecipeManager rejects Recipe with non-positive Id.
    public void Constructor_RejectsNonPositiveId()
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
    public void Constructor_RejectsBlankTitle()
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
    public void Constructor_RejectsDuplicateId()
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

        var result = manager.RemoveRecipeFromCookingPlan(50);
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

        Assert.Equal(1, manager.RemovedRecipeCount);
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

        Assert.Equal(1, manager.RemovedRecipeCount);
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

        // RemoveRecipe does not impact the copy.
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
}
