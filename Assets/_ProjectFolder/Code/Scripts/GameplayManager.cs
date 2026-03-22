using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayManager : SingletonBasic<GameplayManager>
{
    [SerializeReference] private Camera _camera;
    [SerializeReference] private Canvas _canvas;
    [SerializeReference] private RectTransform _dragArea;

    public Camera Camera => _camera;
    public Canvas Canvas => _canvas;
    public RectTransform DragArea => _dragArea;
    public CardTransform Selected { get; private set; }

    public bool IsDraggingObject => Selected != null;
    public Action<bool> onObjectSelectedChanged;

    public Vector3 ScreenToWorldPoint(Vector2 screen) =>
        _camera.ScreenToWorldPoint(new(screen.x, screen.y, _canvas.planeDistance));

    public void SelectCard(CardTransform card)
    {
        Selected = card;
        onObjectSelectedChanged?.Invoke(!IsDraggingObject);
    }
}