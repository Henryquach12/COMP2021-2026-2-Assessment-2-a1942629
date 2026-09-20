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
    // Readonly prevents these collection fields from being reassigned to new collections.
    private readonly Dictionary<int, Recipe> _recipes;
    private readonly LinkedList<int> _cookingPlan;
    private readonly List<string> _shoppingList;
    private readonly Stack<int> _removedRecipes;
    private readonly Queue<string> _cookingInstructions;

    // Verify if the recipe is null.
    private void ValidateNotNullRecipe(Recipe recipe)
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
        _shoppingList = new List<string>();
        _removedRecipes = new Stack<int>();
        _cookingInstructions = new Queue<string>();

        if (recipes is null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }
 
        foreach (Recipe recipe in recipes)
        {
            // Verify each recipe and add them if valid.
            ValidateNotNullRecipe(recipe);
            ValidateIdPositive(recipe);
            ValidateTitleNotBlank(recipe);
            ValidateIdNotDuplicate(recipe);

            _recipes.Add(recipe.Id, recipe);
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _cookingInstructions.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        ValidateNotNullRecipe(recipe);

        try
        {
            ValidateIdPositive(recipe);
            ValidateTitleNotBlank(recipe);
            ValidateIdNotDuplicate(recipe);
        } 
        catch (ArgumentException)
        {
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

        return null;
    }

    public bool RemoveRecipe(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe == null)
        {
            return false;
        }

        else if (_cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _recipes.Remove(recipeId);

        return true;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe == null)
        {
            return 0;
        }
        
        foreach (string ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
        }

        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        // Return a list copy of _shoppingList to prevent caller from modifying the internal _shoppingList.
        return new List<string>(_shoppingList);
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe == null || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);

        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!_cookingPlan.Remove(recipeId))
        {
            return false;
        }

        _removedRecipes.Push(recipeId);

        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (RemovedRecipeCount == 0)
        {
            return false;
        }

        // Peek first so the recipe remains in the stack if it cannot be restored.
        int lastId = _removedRecipes.Peek();

        if (FindRecipe(lastId) == null || _cookingPlan.Contains(lastId))
        {
            return false;
        }

        int removedId = _removedRecipes.Pop();
        _cookingPlan.AddLast(removedId);

        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (RemovedRecipeCount == 0)
        {
            return null;
        }

        return _removedRecipes.Peek();
    }
    
    public IReadOnlyList<int> GetCookingPlan()
    {
        LinkedListNode<int>? currentNode = _cookingPlan.First;

        // List copy of _cookingPlan to prevent caller from modifying the internal _cookingPlan.
        List<int> cookingPlanCopy = [];

        while (currentNode != null)
        {
            cookingPlanCopy.Add(currentNode.Value);   
            currentNode = currentNode.Next;
        }

        return cookingPlanCopy;
    }

    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe == null || recipe.Instructions.Count == 0)
        {
            return false;
        }
        
        _cookingInstructions.Clear();

        foreach (string instruction in recipe.Instructions)
        {
            _cookingInstructions.Enqueue(instruction);
        }
        
        return true;
    }

    public string? PeekNextInstruction()
    {
        if (PendingInstructionCount == 0)
        {
            return null;
        }

        return _cookingInstructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (PendingInstructionCount == 0)
        {
            return null;
        }
        
        return _cookingInstructions.Dequeue();
    }

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
