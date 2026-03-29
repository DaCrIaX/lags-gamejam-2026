using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;

public class RecipeCheckerHandler : HOVCardsGroupHandler
{
    [SerializeReference] private SO_Database _database;
    [SerializeField] private TweenGroup _uiAnimation;
    [SerializeField] private AudioEmitter _sendAudio, _successAudio, _failAudio;

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
        FindIngredients(out var ingredients); print(ingredients.Count);
        if (ingredients.Count <= 2) return;
        bool foundMatch = false;
        int score = 0;

        foreach (var recipe in _database.Recipes)
        {
            int newScore = recipe.GetMatchCount(ingredients);
            if (newScore > score) score = newScore;

            if (recipe.IsMatch(ingredients))
            {
                print($"<color=green>Combinación Exacta: {recipe.name}</color>");
                RemoveIngredientsFromInventory(ingredients);
                _roundManager?.DiscoverRecipe(recipe);
                _successAudio.PlayOneShot();
                foundMatch = true;   
                break;
            }
        }

        _group?.ClearChildren();
        _sendAudio.PlayOneShot();
        _roundManager.SendedIngredients(score * 100);

        if (!foundMatch)
        {
            print("no match found");
            _failAudio.PlayOneShot();
            _roundManager?.NextRound();
        }
        else
        {
            StartCoroutine(NextRoundRoutine());
        }
    }

    private IEnumerator NextRoundRoutine()
    {
        _uiAnimation.DisableGroup();
        yield return new WaitForSeconds(_manager.PreviewNewRecipeDuration + 0.5f);

        _roundManager?.NextRound();
        _uiAnimation.EnableGroup();
    }
}