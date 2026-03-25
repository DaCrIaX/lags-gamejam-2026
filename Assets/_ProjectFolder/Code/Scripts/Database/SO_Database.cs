using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "cards/database")]
public class SO_Database : ScriptableObject
{
    [SerializeReference] private SO_IngredientNormal[] _normalIngredients;
    [SerializeReference] private SO_IngredientExtra[] _extraIngredients;
    [SerializeReference] private SO_IngredientSpecial[] _specialIngredients;

    [SerializeReference] private SO_Recipe[] _recipese;

    public SO_Recipe[] Recipes => _recipese;

    public SO_IngredientBase GetRandomIngredient()
    {
        int value = Random.Range(0, _normalIngredients.Length);
        return _normalIngredients[value];
    }
}