using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[SelectionBase]
public class Card : MonoBehaviour
{
    [SerializeReference] private Image _image;
    [SerializeReference] private SO_IngredientBase _ingredient;

    private CardTransform _transform;

    public CardTransform Transform => _transform;
    public SO_IngredientBase Ingredient => _ingredient;

    private void Awake() => _transform = GetComponentInChildren<CardTransform>();
    private void OnValidate() => Setup(_ingredient);

    public void Setup(SO_IngredientBase ingredient)
    {
        if (ingredient == null) return;

        _ingredient = ingredient;
        _image?.SetImage(ingredient.Image);
        _image?.SetMaterial(ingredient.Material);
    }
}