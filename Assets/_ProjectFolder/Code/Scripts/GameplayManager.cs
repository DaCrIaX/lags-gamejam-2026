using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayManager : SingletonBasic<GameplayManager>
{
    [SerializeReference] private Camera _camera;
    [SerializeReference] private Canvas _canvas;
    [SerializeReference] private RectTransform _dragArea;

    [Header("Gameplay")]
    [SerializeField] private float _startDelay;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private int _startAmount;
    [SerializeField] private int _roundAmount;
    [SerializeField] private float _compareCardsRecipeTime;

    [Header("Cards Movement In Layout")]
    [SerializeField] private float _lerpTimeInLayout;
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private AnimationCurve _movementCurve;

    [Header("Cards On Mouse Hover")]
    [SerializeField] private float _amplitude = 2.5f;
    [SerializeField] private float _frequency = 1f;
    [SerializeField] private float _deltaSpeed = 10f;
    [SerializeField] private float _offset = 2.5f;

    [Header("Cards Drag Movement")]
    [SerializeField] private float _dragVelocity = 10f;
    [SerializeField] private float _dragDeltaMultiply = 5f;

    public Camera Camera => _camera;
    public Canvas Canvas => _canvas;
    public RectTransform DragArea => _dragArea;
    public CardTransform Selected { get; private set; }

    public float CompareCardsRecipeTime => _compareCardsRecipeTime;
    public float StartDelay => _startDelay;
    public float SpawnDelay => _spawnDelay;
    public int StartAmount => _startAmount;
    public int RoundAmount => _roundAmount;

    public float LerpTimeInLayout => _lerpTimeInLayout;
    public float LerpSpeed => _lerpSpeed;
    public AnimationCurve MovementCurve => _movementCurve;

    public float Amplitude => _amplitude;
    public float Frequency => _frequency;
    public float DeltaSpeed => _deltaSpeed;
    public float Offset => _offset;

    public float DragVelocity => _dragVelocity;
    public float DragDeltaMultiply => _dragDeltaMultiply;

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