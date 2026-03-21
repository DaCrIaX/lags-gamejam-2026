using System;
using UnityEngine;

public class CardHolderHorizontal : MonoBehaviour
{
    [Serializable] private struct CardAnimation
    {
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float min, max;

        public float Evaluate(float time) => Mathf.Lerp(min, max, curve.Evaluate(time));
    }

    [SerializeField] private RectTransform _dragSurface;
    [SerializeField] private CardAnimation _position, _rotation;

    private Card[] _cards;
    private Card _selected;
    private float _inverseLength;

    private void Awake() => RefreshCardsArray();
    private void OnTransformChildrenChanged()
    {
        if (_cards.Length != transform.childCount)
            RefreshCardsArray();
    }

    public void RefreshCardsArray()
    {
        _cards = GetComponentsInChildren<Card>();
        _inverseLength = 1f / _cards.Length;
        _inverseLength += _inverseLength * 0.5f;
    }
    public float GetPosition(int index) => _position.Evaluate(_cards.Length <= 1 ? 0.5f : index * _inverseLength);
    public float GetRotation(int index) => _rotation.Evaluate(_cards.Length <= 1 ? 0.5f : index * _inverseLength);

    public void OnBeginDrag(Card card)
    {
        _selected = card;
        _selected.SetContainer(_dragSurface);
    }
    public void OnDragElement(Vector2 position)
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            if (_cards[i] == _selected) continue;

            float siblingX = _cards[i].GetSiblingPosition.x;
            int selectedIdx = _selected.SiblingIndex;
            int siblingIdx = _cards[i].SiblingIndex;

            if (position.x > siblingX && selectedIdx < siblingIdx)
                _selected.SiblingIndex = siblingIdx;
            else if (position.x < siblingX && selectedIdx > siblingIdx)
                _selected.SiblingIndex = siblingIdx;
        }
    }
    public void OnEndDragElement(Vector2 position)
    {
        _selected.ResetContainer();
        _selected = null;
    }
}