using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "cards/database")]
public class SO_Database : ScriptableObject
{
    [SerializeReference] private SO_IngredientBase[] _ingredients;
    [SerializeReference] private SO_Combination[] _combinations;

    public SO_IngredientBase GetRandomIngredient()
    {
        int value = Random.Range(0, _ingredients.Length);
        return _ingredients[value];
    }
}