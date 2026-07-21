using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Animations;

public class RecipeCheckerHandler : HOVCardsGroupHandler
{
    [SerializeField] private SO_Database _database;
    [SerializeField] private SO_ScoringConfig _scoringConfig;
    [SerializeField] private TweenGroup _groupAnimation;
    [SerializeField] private CardsSpawnHandler _cardsSpawnHandler;
    [SerializeField] private AudioEmitterID _audio;
    [SerializeField] private RequestCustomer _requestCustomer;

    private DishEvaluator _dishEvaluator;
    public event Action<DishEvaluationResult> onPlateEvaluated;

    protected override void Awake()
    {
        base.Awake();

        if (_cardsSpawnHandler == null)
        {
            _cardsSpawnHandler = FindObjectOfType<CardsSpawnHandler>(true);
        }
    }
    private void OnEnable()
    {
        if (_dishEvaluator == null)
        {
            _dishEvaluator = new DishEvaluator(_scoringConfig, _database);
        }

        if (_cardsSpawnHandler != null)
        {
            _cardsSpawnHandler.onCardsSpawned += OnCardsSpawned;
        }
    }

    private void OnDisable()
    {
        if (_cardsSpawnHandler != null)
        {
            _cardsSpawnHandler.onCardsSpawned -= OnCardsSpawned;
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
        SetInteractionEnabled(false);
        FindIngredients(out var ingredients);
        var evaluationResult = _dishEvaluator.EvaluateDish(ingredients);
        ApplyRequestScoreRule(evaluationResult, ingredients);
        onPlateEvaluated?.Invoke(evaluationResult);
        ProcessEvaluationResult(evaluationResult, ingredients);
    }
    
    private void ApplyRequestScoreRule(DishEvaluationResult result, Dictionary<SO_IngredientBase, int> ingredients)
    {
        RequestCustomer requestCustomer = GetRequestCustomer();
        if (result == null ||
            result.Type == DishEvaluationResult.DishType.InsufficientCards ||
            requestCustomer == null ||
            requestCustomer.DoesPlateMatchCurrentRequest(ingredients))
        {
            return;
        }

        Debug.Log("No contiene el ingrediente pedido. Puntaje reducido a 30.");
        result.Score = 30;
    }

    private RequestCustomer GetRequestCustomer()
    {
        if (_requestCustomer == null)
        {
            _requestCustomer = FindObjectOfType<RequestCustomer>(true);
        }

        return _requestCustomer;
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

        // Retornar al round sin completarlo
        _roundManager?.CompleteRound();
    }

    private void HandleInsufficientCards(DishEvaluationResult result)
    {
        Debug.Log($"{result.Description}");

        _audio.PlayOneShot("Failure");

        // Retornar al round sin completarlo
        _roundManager?.CompleteRound();
    }

    private IEnumerator NextRoundRoutine()
    {
        yield return new WaitForSeconds(_manager.PreviewNewRecipeDuration + 0.5f);
        _roundManager?.CompleteRound();
    }

    private void OnCardsSpawned(int spawnedAmount)
    {
        SetInteractionEnabled(true);
    }

    private void SetInteractionEnabled(bool enabled)
    {
        _manager?.SetCardInteractionEnabled(enabled);

        if (_groupAnimation == null)
        {
            return;
        }

        if (enabled)
        {
            _groupAnimation.EnableGroup();
            return;
        }

        _groupAnimation.DisableGroup();
    }
}
