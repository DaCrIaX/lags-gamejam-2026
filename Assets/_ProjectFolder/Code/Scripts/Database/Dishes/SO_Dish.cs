using UnityEngine;

[CreateAssetMenu(fileName = "Plate", menuName = "cards/dish/plate")]
public class SO_Dish : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _image;

    [SerializeReference] private SO_IngredientBase[] _ingredients;
}