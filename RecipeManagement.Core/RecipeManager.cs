using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

public sealed class RecipeManager : IRecipeManager
{
    // Readonly prevents these collection fields from being reassigned to new collections.
    private readonly Dictionary<int, Recipe> _recipes;
    private readonly LinkedList<int> _cookingPlan;
    private readonly List<string> _shoppingList;
    private readonly Stack<int> _removedPlanIds;
    private readonly Queue<string> _cookingInstructions;
    private readonly HashSet<int> _savedRecipeIds;

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        _recipes = new Dictionary<int, Recipe>();
        _cookingPlan = new LinkedList<int>();
        _shoppingList = new List<string>();
        _removedPlanIds = new Stack<int>();
        _cookingInstructions = new Queue<string>();
        _savedRecipeIds = new HashSet<int>();

        if (recipes is null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }

        foreach (Recipe recipe in recipes)
        {
            ValidateRecipeNotNull(recipe);
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
    public int RemovedRecipeCount => _removedPlanIds.Count;

    public bool AddRecipe(Recipe recipe)
    {
        ValidateRecipeNotNull(recipe);

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

        if (recipe is null || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _recipes.Remove(recipeId);

        return true;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe is null)
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

        if (recipe is null || _cookingPlan.Contains(recipeId))
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

        _removedPlanIds.Push(recipeId);

        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (RemovedRecipeCount == 0)
        {
            return false;
        }

        int lastId = _removedPlanIds.Pop();

        if (FindRecipe(lastId) is null || _cookingPlan.Contains(lastId))
        {
            return false;
        }

        _cookingPlan.AddLast(lastId);

        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (RemovedRecipeCount == 0)
        {
            return null;
        }

        return _removedPlanIds.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        LinkedListNode<int>? currentNode = _cookingPlan.First;

        // List copy of _cookingPlan to prevent caller from modifying the internal _cookingPlan.
        List<int> cookingPlanCopy = new List<int>();

        while (currentNode is not null)
        {
            cookingPlanCopy.Add(currentNode.Value);
            currentNode = currentNode.Next;
        }

        return cookingPlanCopy;
    }

    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe is null || recipe.Instructions.Count == 0)
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

    public IReadOnlyList<Recipe> SearchByTitle(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return [];
        }
        
        string trimSearchText = searchText.Trim();

        return _recipes.Values
            .Where(recipe => recipe.Title
                .Contains(trimSearchText, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return [];
        }

        string trimSearchText = searchText.Trim();

        return _recipes.Values
            .Where(recipe => recipe.Ingredients
                .Any(ingredient => ingredient.Contains(
                    trimSearchText, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count)
    {
        if (count <= 0)
        {
            return [];
        }

        return _recipes.Values
            .Where(recipe => recipe.Nutrition?.ProteinG is not null)
            .OrderByDescending(recipe => recipe.Nutrition!.ProteinG)
            .Take(count)
            .ToList();
    }

    public bool AddSavedRecipe(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe is null)
        {
            return false;
        }

        return _savedRecipeIds.Add(recipeId);
    }

    public bool RemoveSavedRecipe(int recipeId)
    {
        return _savedRecipeIds.Remove(recipeId);
    }

    public bool IsRecipeSaved(int recipeId)
    {
        return _savedRecipeIds.Contains(recipeId);
    }

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");

    private static void ValidateRecipeNotNull(Recipe recipe)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException(
                nameof(recipe),
                "Recipe cannot be null."
                );
        }
    }

    private static void ValidateIdPositive(Recipe recipe)
    {
        if (recipe.Id <= 0)
        {
            throw new ArgumentException(
                "Recipe Id must be positive.",
                nameof(recipe)
                );
        }
    }

    private static void ValidateTitleNotBlank(Recipe recipe)
    {
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            throw new ArgumentException(
                "Recipe title cannot be blank.",
                nameof(recipe)
                );
        }
    }

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
}
