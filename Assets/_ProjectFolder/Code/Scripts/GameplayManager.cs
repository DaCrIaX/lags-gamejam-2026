using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayManager : SingletonBasic<GameplayManager>
{
    [SerializeField] private RectTransform _dragArea;

    public CardTransform Selected { get; private set; }
    public RectTransform DragArea => _dragArea;

    public bool IsDraggingObject => Selected != null;
    public Action<bool> onObjectSelectedChanged;

    public void SelectCard(CardTransform card)
    {
        Selected = card;
        onObjectSelectedChanged?.Invoke(!IsDraggingObject);
    }
}