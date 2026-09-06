using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    // Test initialise RecipeManager.
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
    // Test RecipeManager rejects null recipe in Recipe list.
    public void Constructor_RejectNullRecipeInList()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(new Recipe[] { null }));
    }

    [Fact]
    // Test RecipeManager rejects non-possitive id
    public void Constructor_RejectNonePossitiveID()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]{new Recipe{
            Id = -20,
            Title = "Recipe A",
            Ingredients = new() { "1 apple" },
            Instructions = new() { "First step", "Second step" }
            }}));
    }

    [Fact]
    // Test RecipeManager rejects blank title
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
    // Test RecipeManager rejects duplicate Id
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
}
