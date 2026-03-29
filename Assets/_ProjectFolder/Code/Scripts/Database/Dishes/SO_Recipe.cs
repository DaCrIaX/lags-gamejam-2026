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

    public bool IsMatch(Dictionary<SO_IngredientBase, int> plateIngredients)
    {
        if (plateIngredients.Count < _ingredients.Count) return false;

        foreach (var req in _ingredients)
        {
            if (!plateIngredients.ContainsKey(req.ingredient) || plateIngredients[req.ingredient] != req.amount)
                return false;
        }

        return plateIngredients.Sum(x => x.Value) == _ingredients.Sum(x => x.amount);
    }

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
}