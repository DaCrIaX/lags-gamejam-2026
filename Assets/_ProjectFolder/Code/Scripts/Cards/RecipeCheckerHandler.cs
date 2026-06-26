using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Animations;

public class RecipeCheckerHandler : HOVCardsGroupHandler
{
    [SerializeField] private SO_Database _database;
    [SerializeField] private SO_ScoringConfig _scoringConfig;
    [SerializeField] private TweenGroup _groupAnimation;
    [SerializeField] private AudioEmitterID _audio;

    private DishEvaluator _dishEvaluator;

    private void OnEnable()
    {
        if (_dishEvaluator == null)
        {
            _dishEvaluator = new DishEvaluator(_scoringConfig, _database);
        }
    }

    private void FindIngredients(out Dictionary<SO_IngredientBase, int> ingredients)
    {
        var cards = _group.GetComponentsInChildren<Card>();
        ingredients = new();

        foreach (var card in cards)
        {
            if (!ingredients.ContainsKey(card.Ingredient))
                ingredients[card.Ingredient] = 1;
            else
                ingredients[card.Ingredient]++;
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
        var evaluationResult = _dishEvaluator.EvaluateDish(ingredients);

        ProcessEvaluationResult(evaluationResult, ingredients);
    }

    private void ProcessEvaluationResult(DishEvaluationResult result, Dictionary<SO_IngredientBase, int> ingredients)
    {
        _roundManager?.SetLastEvaluationResult(result);

        _group?.ClearChildren();
        _audio.PlayOneShot("Send");

        switch (result.Type)
        {
            case DishEvaluationResult.DishType.PerfectMatch:
                HandlePerfectMatch(result, ingredients);
                break;

            case DishEvaluationResult.DishType.CommonDish:
                HandleCommonDish(result, ingredients);
                break;

            case DishEvaluationResult.DishType.InvalidDish:
                HandleInvalidDish(result);
                break;

            case DishEvaluationResult.DishType.InsufficientCards:
                HandleInsufficientCards(result);
                break;
        }
    }

    private void HandlePerfectMatch(DishEvaluationResult result, Dictionary<SO_IngredientBase, int> ingredients)
    {
        Debug.Log($"Match Perfecto: {result.Description}");
        Debug.Log($"Puntos: {result.Score} | Sospecha: +{result.SuspicionChange}");

        RemoveIngredientsFromInventory(ingredients);
        _roundManager?.DiscoverRecipe(result.MatchedRecipe);

        // Enviar puntos y cambio de sospecha
        _roundManager?.SendedIngredients(result.Score);
        _roundManager?.UpdateSuspicion(result.SuspicionChange);

        _audio.PlayOneShot("Success");
        _groupAnimation.DisableGroup();

        StartCoroutine(NextRoundRoutine());
    }

    private void HandleCommonDish(DishEvaluationResult result, Dictionary<SO_IngredientBase, int> ingredients)
    {
        Debug.Log($"Platillo Común: {result.Description}");
        Debug.Log($"Puntos: {result.Score} | Sospecha: {result.SuspicionChange}");

        RemoveIngredientsFromInventory(ingredients);

        // Enviar puntos y cambio de sospecha (negativo = reduce sospecha)
        _roundManager?.SendedIngredients(result.Score);
        _roundManager?.UpdateSuspicion(result.SuspicionChange);

        _audio.PlayOneShot("Success");
        _groupAnimation.DisableGroup();

        StartCoroutine(NextRoundRoutine());
    }

    private void HandleInvalidDish(DishEvaluationResult result)
    {
        Debug.Log($"Combinación Inválida: {result.Description}");
        Debug.Log($"Puntos: {result.Score} | Sospecha: +{result.SuspicionChange}");

        // No se remueven ingredientes
        _roundManager?.SendedIngredients(result.Score);
        _roundManager?.UpdateSuspicion(result.SuspicionChange);

        _audio.PlayOneShot("Failure");
        _groupAnimation.DisableGroup();

        // Retornar al round sin completarlo
        _roundManager?.CompleteRound();
        _groupAnimation.EnableGroup();
    }

    private void HandleInsufficientCards(DishEvaluationResult result)
    {
        Debug.Log($"{result.Description}");

        // No se envían puntos ni cambia sospecha
        _audio.PlayOneShot("Failure");
        _groupAnimation.DisableGroup();

        // Retornar al round sin completarlo
        _roundManager?.CompleteRound();
    }

    private IEnumerator NextRoundRoutine()
    {
        yield return new WaitForSeconds(_manager.PreviewNewRecipeDuration + 0.5f);
        _roundManager?.CompleteRound();
        _groupAnimation.EnableGroup();
    }
}
