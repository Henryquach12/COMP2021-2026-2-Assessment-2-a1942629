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
    public void Constructor_BuildsRecipeDictionary()
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
    // Test RecipeManager rejects Recipe with non-positive id
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
    // Test RecipeManager rejects Recipe with blank title
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
    // Test RecipeManager rejects Recipe with duplicate Id
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
    // Test AddRecipe
    public void AddRecipe()
    {
        var manager = CreateManager();
        var recipe = CreateRecipe();
        bool addedRecipe = manager.AddRecipe(recipe);
        Assert.Equal(3, manager.RecipeCount);
        Assert.True(addedRecipe);
    }

    [Fact] 
    // Test AddRecipe rejects null parameter
    public void AddRecipe_RejectNullRecipe()
    {
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(null!));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test AddRecipe rejects non-positive id Recipe
    public void AddRecipe_RejectNonPositiveId()
    {
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(new Recipe{
            Id = -20,
            Title = "Recipe A",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test AddRecipe rejects blank title Recipe
    public void AddRecipe_RejectBlankTitle()
    {
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(new Recipe{
            Id = 20,
            Title = "",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test AddRecipe rejects duplicate Id Recipe
    public void AddRecipe_RejectDuplicateId()
    {
        var manager = CreateManager();
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
    public void FindRecipe_ReturnMatchRecipe()
    {
        var manager = CreateManager();
        Recipe? recipe = manager.FindRecipe(10);
        Assert.NotNull(recipe);
        Assert.Equal(10, recipe.Id);
        Assert.Equal("Recipe A", recipe.Title);
    }

    [Fact] 
    // Test FindRecipe that return null since Recipe is not found.
    public void FindRecipe_ReturnNull()
    {
        var manager = CreateManager();
        Recipe? recipe = manager.FindRecipe(50);
        Assert.Null(recipe);
    }

    [Fact] 
    // Test RemoveRecipe that successfully removes Recipe and returns true.
    public void RemoveRecipe_SuccessfulRemove()
    {
        var manager = CreateManager();
        Assert.True(manager.RemoveRecipe(10));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact] 
    // Test RemoveRecipe returns false with missing Recipe Id.
    public void RemoveRecipe_MissingIdReturnFalse()
    {
        var manager = CreateManager();
        Assert.False(manager.RemoveRecipe(100));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact] 
    // Test RemoveRecipe returns false when Recipe in cooking plan.
    public void RemoveRecipe_RecipInCookingPlanReturnFalse()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        Assert.False(manager.RemoveRecipe(10));
        Assert.Equal(2, manager.RecipeCount);
    }
    
    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
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
