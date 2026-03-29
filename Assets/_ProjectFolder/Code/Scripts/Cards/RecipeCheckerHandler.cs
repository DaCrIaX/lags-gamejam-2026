using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCheckerHandler : HOVCardsGroupHandler
{
    [SerializeReference] private SO_Database _database;

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
        bool foundMatch = false;

        foreach (var recipe in _database.Recipes)
        {
            if (recipe.IsMatch(ingredients))
            {
                print($"<color=green>Combinación Exacta: {recipe.name}</color>");
                RemoveIngredientsFromInventory(ingredients);
                _roundManager?.DiscoverRecipe(recipe);
                foundMatch = true;
                break;
            }
        }

        _group?.ClearChildren();

        if (!foundMatch)
        {
            print("no match found");
        }

        StartCoroutine(NextRoundRoutine());
    }

    private IEnumerator NextRoundRoutine()
    {
        yield return new WaitForSeconds(_manager.CompareCardsRecipeTime);
        _roundManager?.CompleteRound();
    }
}