using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonBasic<InventoryManager>
{
    [SerializeField] private SO_Inventory _inventory;
    private List<SO_IngredientBase> _inUse = new();

    protected override void Awake()
    {
        base.Awake();
        _inventory.Setup();
    }
    public void AddToInventory(SO_IngredientBase ingredient) => _inventory.Add(ingredient);
    public void RemoveFromInUse(SO_IngredientBase ingredient) => _inUse.Remove(ingredient);

    public bool TryGetIngredients(int amount, out List<SO_IngredientBase> ingredients)
    {
        ingredients = new();
        if (_inventory.Count <= 0) return false;

        int itemsToMove = Mathf.Min(amount, _inventory.Count);

        for (int i = 0; i < itemsToMove; i++)
        {
            var random = _inventory.GetRandomIngredient();
            ingredients.Add(random);

            _inUse.Add(ingredients[i]);
            _inventory.Remove(random);
        }

        return true;
    }
}