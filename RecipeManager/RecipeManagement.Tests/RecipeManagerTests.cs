using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    // Test calling RecipeManager constructor.
    public void Constructor_SuccessfullyBuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    // Test RecipeManger rejects null Recipe list.
    public void Constructor_RejectNullRecipesList()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    // Test RecipeManager rejects null Recipe in Recipe list.
    public void Constructor_RejectNullRecipeInList()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(new Recipe[] {null!}));
    }

    [Fact]
    // Test RecipeManager rejects Recipe with non-positive id.
    public void Constructor_RejectNonPositiveId()
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
    public void Constructor_RejectBlankTitle()
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
    public void Constructor_RejectDuplicateId()
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
    // Test AddRecipe successfully addes Recipe.
    public void AddRecipe_SuccessfullyAddRecipe()
    {
        var manager = CreateManager();
        var recipe = CreateRecipe();
        Assert.Equal(2, manager.RecipeCount);
        
        bool addedRecipe = manager.AddRecipe(recipe);
        Assert.Equal(3, manager.RecipeCount);
        Assert.True(addedRecipe);
        
        Recipe? added = manager.FindRecipe(30);
        Assert.NotNull(added);
        Assert.Equal(30, added.Id);
    }

    [Fact] 
    // Test AddRecipe rejects null parameter.
    public void AddRecipe_RejectNullRecipe()
    {
        var manager = CreateManager();
        
        Assert.Equal(2, manager.RecipeCount);
        Assert.False(manager.AddRecipe(null!));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test AddRecipe rejects non-positive id Recipe.
    public void AddRecipe_RejectNonPositiveId()
    {
        var manager = CreateManager();
        
        Assert.Equal(2, manager.RecipeCount);
        Assert.False(manager.AddRecipe(new Recipe{
            Id = -20,
            Title = "Recipe A",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test AddRecipe rejects blank title Recipe.
    public void AddRecipe_RejectBlankTitle()
    {
        var manager = CreateManager();
        
        Assert.Equal(2, manager.RecipeCount);
        Assert.False(manager.AddRecipe(new Recipe{
            Id = 20,
            Title = "",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test AddRecipe rejects duplicate Id Recipe.
    public void AddRecipe_RejectDuplicateId()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        
        Assert.False(manager.AddRecipe(new Recipe{
            Id = 10,
            Title = "Recipe D",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test FindRecipe that return match Recipe.
    public void FindRecipe_SuccessfullyReturnMatchRecipe()
    {
        var manager = CreateManager();
        
        Recipe? recipe = manager.FindRecipe(10);
        Assert.NotNull(recipe);
        Assert.Equal(10, recipe.Id);
        Assert.Equal("Recipe A", recipe.Title);
    }

    [Fact] 
    // Test FindRecipe that return null since Recipe is not found.
    public void FindRecipe_RejectMissingRecipeId()
    {
        var manager = CreateManager();

        Recipe? recipe = manager.FindRecipe(50);
        Assert.Null(recipe);
    }

    [Fact] 
    // Test RemoveRecipe that successfully removes Recipe and returns true.
    public void RemoveRecipe_SuccessfullyRemove()
    {
        var manager = CreateManager();
        
        Assert.Equal(2, manager.RecipeCount);
        Assert.True(manager.RemoveRecipe(10));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact] 
    // Test RemoveRecipe returns false with missing Recipe Id.
    public void RemoveRecipe_RejectMissingId()
    {
        var manager = CreateManager();
        
        Assert.Equal(2, manager.RecipeCount);
        Assert.False(manager.RemoveRecipe(100));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test RemoveRecipe returns false when Recipe in cooking plan.
    public void RemoveRecipe_RejectRecipeInCookingPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        
        Assert.Equal(2, manager.RecipeCount);
        Assert.False(manager.RemoveRecipe(10));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test AddIngredientsToShoppingList returns the correct count of those required ingredients.
    public void AddIngredientsToShoppingList_SuccessfullyAddIngredients()
    {
        var manager = CreateManager();
        Assert.Equal(0, manager.ShoppingItemCount);

        int numberOfIngredients = manager.AddIngredientsToShoppingList(10);
        Assert.Equal(2, numberOfIngredients);
        Assert.Equal(2, manager.ShoppingItemCount);

        var shoppingList = manager.GetShoppingList();
        Assert.Equal("1 apple", shoppingList[0]);
        Assert.Equal("2 banana", shoppingList[1]);
    }

    [Fact] 
    // Test AddIngredientsToShoppingList returns 0 when recipeId is missing.
    public void AddIngredientsToShoppingList_MissingRecipeIdReturn0()
    {
        var manager = CreateManager();
        Assert.Equal(0, manager.ShoppingItemCount);

        int numberOfIngredients = manager.AddIngredientsToShoppingList(100);
        Assert.Equal(0, numberOfIngredients);
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact] 
    // Test GetShoppingList returns ingredient list without exposing internal list.
    public void GetShoppingList_ReturnIngredientListWithoutExposing()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        
        var result = manager.GetShoppingList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1 apple", result[0]);
        Assert.Equal("2 banana", result[1]);

        // Verify that the returned list is a separate copy.
        manager.ClearShoppingList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1 apple", result[0]);
        Assert.Equal("2 banana", result[1]);
    }

    [Fact] 
    // Test ClearShoppingList removes all the items in the current shopping list. 
    public void ClearShoppingList_SuccessfullyRemoveShoppingItems()
    {
        var manager = CreateManager();

        manager.AddIngredientsToShoppingList(10);
        manager.ClearShoppingList();
        var result = manager.GetShoppingList();

        Assert.Empty(result);
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact] 
    // Test AddRecipeToCookingPlan successfully add recipe to cooking plan. 
    public void AddRecipeToCookingPlan_SuccessfullyAddRecipeToPlan()
    {
        var manager = CreateManager();
        var result = manager.AddRecipeToCookingPlan(10);

        Assert.True(result);
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact] 
    // Test AddRecipeToCookingPlan rejects non-existing recipe.
    public void AddRecipeToCookingPlan_RejectNullRecipe()
    {
        var manager = CreateManager();
        var result = manager.AddRecipeToCookingPlan(100);

        Assert.False(result);
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact] 
    // Test AddRecipeToCookingPlan rejects duplicate recipe in plan.
    public void AddRecipeToCookingPlan_RejectDuplicateRecipe()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        var result = manager.AddRecipeToCookingPlan(10);

        Assert.False(result);
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact] 
    // Test RemoveRecipeFromCookingPlan successfully removes recipe from the cooking plan.
    public void RemoveRecipeFromCookingPlan_SuccessfullyRemoveRecipe()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        var result = manager.RemoveRecipeFromCookingPlan(10);

        Assert.True(result);
        Assert.Equal(0, manager.CookingPlanCount);
        Assert.Equal(1, manager.RemovedRecipeCount);
    }

    [Fact] 
    // Test RemoveRecipeFromCookingPlan rejects recipe Id not in cooking plan.
    public void RemoveRecipeFromCookingPlan_RejectRecipeIdNotInCookingPlan()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        var result = manager.RemoveRecipeFromCookingPlan(50);

        Assert.False(result);
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact] 
    // Test RemoveRecipeFromCookingPlan successfully restore last removed Recipe.
    public void RestoreLastRemovedRecipe_SuccessfullyRestoreLastRemovedRecipe()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(0, manager.CookingPlanCount);
        Assert.Equal(1, manager.RemovedRecipeCount);

        bool result = manager.RestoreLastRemovedRecipe();

        Assert.True(result);
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    // Test RestoreLastRemovedRecipe returns false when there is no removed Recipe.
    public void RestoreLastRemovedRecipe_RejectNoRemovedRecipe()
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
    public void RestoreLastRemovedRecipe_RejectRecipeNoLongerExists()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);

        manager.RemoveRecipe(10);

        Assert.Null(manager.FindRecipe(10));

        bool result = manager.RestoreLastRemovedRecipe();

        Assert.False(result);
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    // Test RestoreLastRemovedRecipe returns false when the removed Recipe is still in the cooking plan.
    public void RestoreLastRemovedRecipe_RejectRecipeInCookingPlan()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(0, manager.CookingPlanCount);

        manager.AddRecipeToCookingPlan(10);

        bool result = manager.RestoreLastRemovedRecipe();

        Assert.False(result);
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    // Test PeekLastRemovedRecipe returns null when the removed recipe stack is empty.
    public void PeekLastRemovedRecipe_RejectEmptyStack()
    {
        var manager = CreateManager();

        Assert.Equal(0, manager.RemovedRecipeCount);

        int? result = manager.PeekLastRemovedRecipe();

        Assert.Null(result);
    }

    [Fact]
    // Test PeekLastRemovedRecipe successfully returns last removed recipe Id.
    public void PeekLastRemovedRecipe_SuccessfullyFindLastRemoveRecipeId()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(30);
        manager.AddRecipeToCookingPlan(40);

        manager.RemoveRecipeFromCookingPlan(30);
        manager.RemoveRecipeFromCookingPlan(40);

        Assert.Equal(2, manager.RemovedRecipeCount);

        int? result = manager.PeekLastRemovedRecipe();

        Assert.Equal(40, result);
        Assert.Equal(2, manager.RemovedRecipeCount);
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
        return new Recipe{
                Id = 30,
                Title = "Recipe C",
                Ingredients = new() { "2 apple" },
                Instructions = new() { "First step", "Third step" }
            };
    }
}
