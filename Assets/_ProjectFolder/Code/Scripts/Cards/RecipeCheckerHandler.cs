using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCheckerHandler : HOVCardsGroupHandler
{
    [SerializeReference] private SO_Database _database;
    [SerializeField, Range(1f, 10f)] private float _checkingTime = 1f;

    private void FindIngredients(out Dictionary<SO_IngredientBase, int> ingredients)
    {
        var cards = _group.GetComponentsInChildren<Card>();
        ingredients = new();

        foreach (var card in cards)
        {
            if (!ingredients.ContainsKey(card.Ingredient)) ingredients[card.Ingredient] = 1;
            else ingredients[card.Ingredient]++;
        }
    }
    private void RemoveIngredientsFromInventory(Dictionary<SO_IngredientBase, int> ingredients)
    {
        foreach (var item in ingredients)
        {
            for (int i = 0; i < item.Value; i++)
                _inventory.RemoveFromInUse(item.Key);
        }
    }

    public void EvaluatePlate()
    {
        FindIngredients(out var ingredients);
        if (ingredients.Count == 0) return;

        foreach (var recipe in _database.Recipes)
        {
            if (recipe.IsMatch(ingredients))
            {
                RemoveIngredientsFromInventory(ingredients);
                StartCoroutine(NextRoundRoutine(recipe));
                return;
            }
        }

        print("No hay receta válida con estos ingredientes exactos.");
    }

    private IEnumerator NextRoundRoutine(SO_Recipe recipe)
    {
        _group?.ClearChildren();
        _roundManager?.DiscoverRecipe(recipe);

        print($"<color=green>Combinación Exacta: {recipe.name}</color>");
        yield return new WaitForSeconds(_checkingTime);
        
        _roundManager?.CompleteRound();
    }
}