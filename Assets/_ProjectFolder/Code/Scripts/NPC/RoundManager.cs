using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private CardManager _cardManager;
    [SerializeField] private TweenGroup _canvas;

    [SerializeField] private CardGroup _recipe;
    [SerializeField] private SplineContainer _spline;

    private void Start()
    {
        _canvas.EnableGroup();
        _cardManager.AddNewCards();
    }
    public void SendRecipe()
    {
        var cards = _recipe.GetComponentsInChildren<Card>();

        _recipe.ClearChildren();
        _cardManager.AddNewCards();
    }
}