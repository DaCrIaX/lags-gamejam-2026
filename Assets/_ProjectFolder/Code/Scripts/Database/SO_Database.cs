using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "cards/database")]
public class SO_Database : ScriptableObject
{
    [SerializeReference] private SO_IngredientBase[] _ingredients;
    [SerializeReference] private SO_IngredientSpecial[] _specialIngredients;

    [SerializeReference] private SO_Dish[] _combinations;

    public SO_IngredientBase GetRandomIngredient()
    {
        int value = Random.Range(0, _ingredients.Length);
        return _ingredients[value];
    }
}