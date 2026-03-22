using UnityEngine;

public abstract class SO_IngredientBase : ScriptableObject
{
    [SerializeReference] private Sprite _image;
    [SerializeField] private string _name;

    [Space]
    [SerializeReference] private Material _material;

    public Sprite Image => _image;
    public string Name => _name;

    public Material Material => _material;
}