using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonBasic<InventoryManager>
{
    [SerializeField] private List<SO_IngredientBase> _availableItems = new();
    private List<SO_IngredientBase> _inUse = new();

    public bool HasItems => _availableItems.Count > 0;

    public void AddToInventory(SO_IngredientBase ingredient) => _availableItems.Add(ingredient);
    public void RemoveFromInUse(SO_IngredientBase ingredient) => _inUse.Remove(ingredient);

    public bool TryGetIngredients(int amount, out List<SO_IngredientBase> ingredients)
    {
        ingredients = new();
        if (!HasItems) return false;

        int itemsToMove = Mathf.Min(amount, _availableItems.Count);

        for (int i = 0; i < itemsToMove; i++)
        {
            int randomIndex = Random.Range(0, _availableItems.Count);
            ingredients.Add(_availableItems[randomIndex]);

            _inUse.Add(ingredients[i]);
            _availableItems.RemoveAt(randomIndex);
        }

        return true;
    }
}