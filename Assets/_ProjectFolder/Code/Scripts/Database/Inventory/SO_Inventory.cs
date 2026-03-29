using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "InventoryData", menuName = "cards/inventory")]
public class SO_Inventory : ScriptableObject
{
    [SerializeField] private int _defaultAmount;
    [SerializeField] private SO_IngredientBase _testing;
    [SerializeField] private SerializedDictionary<SO_IngredientBase, int> _ingredients = new();

    public int Count => _ingredients.Count(pair => pair.Value > 0);
    public IReadOnlyDictionary<SO_IngredientBase, int> Ingredients => _ingredients;

    public void Setup()
    {
        var keys = new List<SO_IngredientBase>(_ingredients.Keys);

        foreach (var key in keys)
            _ingredients[key] = _defaultAmount;
    }

    [ContextMenu("Add")]
    public void Add() => _ingredients.Add(_testing, _defaultAmount);
    public void Add(SO_IngredientBase ingredient)
    {
        if (_ingredients.ContainsKey(ingredient))
            _ingredients[ingredient]++;
        else
            _ingredients[ingredient] = 1;
    }

    [ContextMenu("Remove")]
    public void Remove() => _ingredients.Remove(_testing);
    public void Remove(SO_IngredientBase ingredient)
    {
        if (_ingredients.ContainsKey(ingredient) && _ingredients[ingredient] > 0)
            _ingredients[ingredient]--;
        else
            _ingredients[ingredient] = 0;
    }

    public SO_IngredientBase GetRandomIngredient()
    {
        var candidatos = _ingredients.Where(pair => pair.Value > 0).Select(pair => pair.Key).ToList();

        if (candidatos.Count > 0) {
            int index = Random.Range(0, candidatos.Count);
            return candidatos[index];
        }

        return null;
    }
}