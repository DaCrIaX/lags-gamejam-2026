using System;
using UnityEngine;
using UnityEngine.Localization;

[Flags] public enum CardType
{
    Normal = 1, Extra = 2, Special = 4
}
public enum IngredientType
{
    Proteina,
    Vegetal,
    Carbohidrato,
    Condimento
}

[Serializable]
public abstract class SO_IngredientBase : ScriptableObject
{
    [SerializeField] private CardType _cardType = CardType.Normal;
    [SerializeField] private IngredientType _type;
    [Space]
    [SerializeField] private LocalizedSprite _sprite;
    [SerializeField] private string _name;

    [Space]
    [SerializeReference] private Material _material;

    public LocalizedSprite Image => _sprite;
    public string Name => _name;

    public CardType CardType => _cardType;
    public IngredientType Type => _type;
    public Material Material => _material;
}