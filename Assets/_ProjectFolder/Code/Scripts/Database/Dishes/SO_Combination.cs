using UnityEngine;

[CreateAssetMenu(fileName = "Combination", menuName = "cards/dish/combination")]
public class SO_Combination : ScriptableObject
{
    [SerializeReference] private SO_IngredientBase _base;
    [SerializeReference] private SO_IngredientBase[] _extra;

    [SerializeReference] private SO_Dish _common, _special, _rare;
}