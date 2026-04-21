using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;

public class RecipeCheckerHandler : HOVCardsGroupHandler
{
    [SerializeReference] private SO_Database _database;
    [SerializeField] private TweenGroup _groupAnimation;
    [SerializeField] private AudioEmitterID _audio;

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

        int count = 0;
        foreach (var item in ingredients) count += item.Value;

        if (count < 3) return;
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
                _audio.PlayOneShot("Success");
                foundMatch = true;   
                break;
            }
        }

        _group?.ClearChildren();
        _audio.PlayOneShot("Send");
        _roundManager.SendedIngredients(score * 100);
        _groupAnimation.DisableGroup();

        if (!foundMatch)
        {
            _audio.PlayOneShot("Failure");
            _roundManager?.CompleteRound();
        }
        else
        {
            StartCoroutine(NextRoundRoutine());
        }
    }

    private IEnumerator NextRoundRoutine()
    {
        yield return new WaitForSeconds(_manager.PreviewNewRecipeDuration + 0.5f);
        _roundManager?.CompleteRound();
    }
}