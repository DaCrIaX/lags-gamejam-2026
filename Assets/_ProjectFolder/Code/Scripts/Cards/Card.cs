using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeReference] private SO_IngredientBase _ingredient;

    private void OnValidate() => Setup(_ingredient);

    public void Setup(SO_IngredientBase ingredient)
    {
        if (ingredient == null) return;

        _ingredient = ingredient;
        _image?.SetImage(ingredient.Image);
        _image?.SetMaterial(ingredient.Material);
    }
}