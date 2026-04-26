using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientRequirement
{
    public SO_IngredientBase ingredient;
    public int amount = 1;
}

[CreateAssetMenu(fileName = "Plate", menuName = "cards/dish/plate")]
public class SO_Recipe : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _image;
    [SerializeField] private List<IngredientRequirement> _ingredients;

    public string Name => _name;
    public Sprite Image => _image;
    public List<IngredientRequirement> Ingredients => _ingredients;

    /// <summary>
    /// Verifica si los ingredientes en el plato coinciden EXACTAMENTE con la receta
    /// </summary>
    public bool IsMatch(Dictionary<SO_IngredientBase, int> plateIngredients)
    {
        if (plateIngredients.Count != _ingredients.Count)
            return false;

        foreach (var req in _ingredients)
        {
            if (!plateIngredients.ContainsKey(req.ingredient) ||
                plateIngredients[req.ingredient] != req.amount)
                return false;
        }

        // Verificar que no hay ingredientes extra
        return plateIngredients.Sum(x => x.Value) == _ingredients.Sum(x => x.amount);
    }

    /// <summary>
    /// Cuenta cuántos requisitos de ingredientes coinciden
    /// (usado para feedback parcial al jugador)
    /// </summary>
    public int GetMatchCount(Dictionary<SO_IngredientBase, int> plateIngredients)
    {
        int currentMatches = 0;

        foreach (var req in _ingredients)
        {
            if (plateIngredients.TryGetValue(req.ingredient, out int amountInPlate))
            {
                if (amountInPlate == req.amount)
                    currentMatches++;
            }
        }

        return currentMatches;
    }

    /// <summary>
    /// Obtiene el tipo de ingrediente más común en esta receta
    /// (útil para análisis y UI)
    /// </summary>
    public IngredientType GetPrimaryIngredientType()
    {
        return _ingredients
            .GroupBy(x => x.ingredient.Type)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }

    /// <summary>
    /// Verifica si todos los ingredientes son del mismo tipo
    /// </summary>
    public bool IsHomogeneousRecipe()
    {
        if (_ingredients.Count == 0) return false;
        var firstType = _ingredients[0].ingredient.Type;
        return _ingredients.All(x => x.ingredient.Type == firstType);
    }
}
