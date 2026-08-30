using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // Readonly prevents the field from being reassigned to a new dictionary.
    private readonly Dictionary<int, Recipe> _recipes;
    private readonly LinkedList<int> _cookingPlan;

    // Verify if the recipe is null.
    private void ValidateNotNull(Recipe recipe)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException(
                nameof(recipe),
                "Recipe cannot be null."
                );
        }
    }

    // Verify if the recipe id is non-positive.
    private void ValidateIdPositive(Recipe recipe)
    {
        if (recipe.Id <= 0)
        {
            throw new ArgumentException(
                "Recipe Id must be positive.",
                nameof(recipe)
                );
        }
    }

    // Verify if the recipe title is blank.
    private void ValidateTitleNotBlank(Recipe recipe)
    {
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            throw new ArgumentException(
                "Recipe title cannot be blank.",
                nameof(recipe)
                );
        }
    }

    // Verify if the recipe Id is duplicate.
    private void ValidateIdNotDuplicate(Recipe recipe)
    {
        if (_recipes.ContainsKey(recipe.Id))
        {
            throw new ArgumentException(
                "Recipe Id cannot be duplicate.",
                nameof(recipe)
                );
        }
    }

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        _recipes = new Dictionary<int, Recipe>();
        _cookingPlan = new LinkedList<int>();

        if (recipes is null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }
 
        // Verify each recipe and add them if valid.
        foreach (Recipe recipe in recipes)
        {
            ValidateNotNull(recipe);
            ValidateIdPositive(recipe);
            ValidateTitleNotBlank(recipe);
            ValidateIdNotDuplicate(recipe);

            _recipes.Add(recipe.Id, recipe);
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => 0;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => 0;
    public int RemovedRecipeCount => 0;

    public bool AddRecipe(Recipe recipe)
    {
        ValidateNotNull(recipe);

        try
        {
            ValidateIdPositive(recipe);
            ValidateTitleNotBlank(recipe);
            ValidateIdNotDuplicate(recipe);
        }
        catch (ArgumentException ex){
            Console.WriteLine($"Invalid recipe: {ex.Message}");
            return false;
        }

        _recipes.Add(recipe.Id, recipe);

        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        if (_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return recipe;
        }

        Console.WriteLine($"{recipeId} cannot be found.");
        return null;
    }

    public bool RemoveRecipe(int recipeId)
    {
        if(!_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            Console.WriteLine($"{recipeId} cannot be found.");
            return false;
        }
        else if (_cookingPlan.Contains(recipeId))
        {
            Console.WriteLine($"{recipeId} {recipe.Title} is currently in the cooking plan.");
            return false;
        }
        _recipes.Remove(recipeId);
        return true;
    }

    public int AddIngredientsToShoppingList(int recipeId) =>
        throw new NotImplementedException("Part A: implement AddIngredientsToShoppingList.");

    public IReadOnlyList<string> GetShoppingList() =>
        throw new NotImplementedException("Part A: implement GetShoppingList.");

    public void ClearShoppingList() =>
        throw new NotImplementedException("Part A: implement ClearShoppingList.");

    public bool AddRecipeToCookingPlan(int recipeId) =>
        throw new NotImplementedException("Part A: implement AddRecipeToCookingPlan.");

    public bool RemoveRecipeFromCookingPlan(int recipeId) =>
        throw new NotImplementedException("Part A: implement RemoveRecipeFromCookingPlan.");

    public bool RestoreLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement RestoreLastRemovedRecipe.");

    public int? PeekLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement PeekLastRemovedRecipe.");

    public IReadOnlyList<int> GetCookingPlan() =>
        throw new NotImplementedException("Part A: implement GetCookingPlan.");

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Part A: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Part A: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Part A: implement CompleteNextInstruction.");

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
