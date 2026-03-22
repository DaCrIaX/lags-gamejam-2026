using UnityEngine;

[CreateAssetMenu(fileName = "Combination", menuName = "cards/dish/combination")]
public class SO_Combination : ScriptableObject
{
    [SerializeReference] private SO_IngredientNormal _base;
    [SerializeReference] private SO_IngredientExtra[] _extras;
    [SerializeReference] private SO_IngredientSpecial[] _specials;

    [SerializeReference] private SO_Dish _common, _special, _rare;
}