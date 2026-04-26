using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DishEvaluationResult
{
    public enum DishType
    {
        PerfectMatch,      // Match exacto con receta
        CommonDish,        // 3+ ingredientes del mismo tipo
        InvalidDish,       // Menos cartas o combinación aleatoria
        InsufficientCards  // Menos de 3 cartas
    }

    public DishType Type { get; set; }
    public int Score { get; set; }
    public int SuspicionChange { get; set; }
    public int CardCount { get; set; }
    public string Description { get; set; }
    public SO_Recipe MatchedRecipe { get; set; }
}

public class DishEvaluator
{
    private SO_ScoringConfig _scoringConfig;
    private SO_Database _database;

    public DishEvaluator(SO_ScoringConfig scoringConfig, SO_Database database)
    {
        _scoringConfig = scoringConfig;
        _database = database;
    }

    /// <summary>
    /// Evalúa un platillo basado en los ingredientes proporcionados
    /// </summary>
    public DishEvaluationResult EvaluateDish(Dictionary<SO_IngredientBase, int> plateIngredients)
    {
        var result = new DishEvaluationResult();
        int cardCount = plateIngredients.Values.Sum();

        result.CardCount = cardCount;

        // Validación: menos de 3 cartas
        if (cardCount < _scoringConfig.MinCardsForValidDish)
        {
            result.Type = DishEvaluationResult.DishType.InsufficientCards;
            result.Score = -10;
            result.SuspicionChange = 5;
            result.Description = $"Insuficientes cartas. Mínimo: {_scoringConfig.MinCardsForValidDish}";
            return result;
        }

        // Búsqueda: ¿existe un match perfecto?
        foreach (var recipe in _database.Recipes)
        {
            if (recipe.IsMatch(plateIngredients))
            {
                result.Type = DishEvaluationResult.DishType.PerfectMatch;
                result.MatchedRecipe = recipe;
                result.Score = CalculatePerfectMatchScore(cardCount);
                result.SuspicionChange = _scoringConfig.PerfectMatchSuspicion;
                result.Description = $"¡Match perfecto! {recipe.Name}";
                return result;
            }
        }

        // Validación: ¿es un platillo común (3+ del mismo tipo)?
        if (IsCommonDish(plateIngredients, out var ingredientType))
        {
            result.Type = DishEvaluationResult.DishType.CommonDish;
            result.Score = CalculateCommonDishScore(cardCount);
            result.SuspicionChange = _scoringConfig.CommonDishSuspicion;
            result.Description = $"Platillo común de {ingredientType}";
            return result;
        }

        // Fallback: combinación aleatoria
        result.Type = DishEvaluationResult.DishType.InvalidDish;
        result.Score = -10;
        result.SuspicionChange = _scoringConfig.InvalidDishSuspicion;
        result.Description = "Combinación de ingredientes aleatoria";
        return result;
    }

    /// <summary>
    /// Calcula puntuación para match perfecto
    /// Base score + bonus por cantidad de cartas
    /// </summary>
    private int CalculatePerfectMatchScore(int cardCount)
    {
        int baseScore = _scoringConfig.PerfectMatchBaseScore;
        int bonusScore = (int)(cardCount * _scoringConfig.ScoreMultiplierPerCard);
        return baseScore + bonusScore;
    }

    /// <summary>
    /// Calcula puntuación para platillo común
    /// Base score + proporción según cantidad de cartas vs mínimo requerido
    /// </summary>
    private int CalculateCommonDishScore(int cardCount)
    {
        float ratio = (float)cardCount / _scoringConfig.MinCardsForValidDish;
        int baseScore = _scoringConfig.CommonDishBaseScore;
        int finalScore = (int)(baseScore * ratio);
        return finalScore;
    }

    /// <summary>
    /// Verifica si es un platillo común (3+ ingredientes del mismo tipo)
    /// </summary>
    private bool IsCommonDish(Dictionary<SO_IngredientBase, int> plateIngredients, out string ingredientType)
    {
        ingredientType = "";
        var groupedByType = plateIngredients
            .GroupBy(x => x.Key.Type)
            .OrderByDescending(g => g.Sum(x => x.Value))
            .FirstOrDefault();

        if (groupedByType != null && groupedByType.Sum(x => x.Value) >= _scoringConfig.MinTypeForCommon)
        {
            ingredientType = groupedByType.Key.ToString();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Obtiene todas las recetas que podrían coincidir parcialmente
    /// (para mostrar feedback al jugador)
    /// </summary>
    public List<(SO_Recipe recipe, int matchCount)> GetPartialMatches(Dictionary<SO_IngredientBase, int> plateIngredients)
    {
        var matches = new List<(SO_Recipe, int)>();

        foreach (var recipe in _database.Recipes)
        {
            int newScore = recipe.GetMatchCount(plateIngredients);
            if (newScore > 0)
            {
                matches.Add((recipe, newScore));
            }
        }

        return matches.OrderByDescending(x => x.Item2).ToList();
    }
}
