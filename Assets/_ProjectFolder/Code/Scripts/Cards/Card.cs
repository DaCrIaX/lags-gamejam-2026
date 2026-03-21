using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.WSA;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Transform _parent, _transform;
    [SerializeField] private float _smoothSpeed = 10f;

    [SerializeField] private CanvasGroup _cast;
    [SerializeField] private Transform _shadow;
    [SerializeField] private float _height;
    [SerializeField] private float _fadeTime = 0.25f;

    private CardHolderHorizontal _holder;
    private bool _isDragging = false;

    private Vector2 _shadowPosition;
    private Tween _shadowTween;

    public int SiblingIndex { get => _parent.GetSiblingIndex(); set => _parent.SetSiblingIndex(value); }
    public Vector2 GetSiblingPosition => _parent.position;

    private void Awake() => RefreshCardHolder();
    private void Start() => _shadowPosition = _shadow.localPosition;
    private void LateUpdate()
    {
        if (!_holder) return;

        int index = SiblingIndex;
        float speed = Time.deltaTime * _smoothSpeed;

        if (!_isDragging)
        {
            var targetLocalPos = new Vector3(0, _holder.GetPosition(index), 0);
            _transform.localPosition = Vector3.Lerp(_transform.localPosition, targetLocalPos, speed);
        }

        var targetRot = Quaternion.Euler(0, 0, -_holder.GetRotation(index));
        _transform.localRotation = Quaternion.Lerp(_transform.localRotation, targetRot, speed);
    }

    public void RefreshCardHolder() => _holder = GetComponentInParent<CardHolderHorizontal>();
    public void SetContainer(Transform parent) => _transform.SetParent(parent);
    public void ResetContainer() => _transform.SetParent(_parent);
    public void SetParent(Transform parent) => _parent.SetParent(parent);

    public void OnBeginDrag(PointerEventData eventData)
    {
        _cast.blocksRaycasts = false;
        _holder?.OnBeginDrag(this);
        _shadowTween.Complete();
        _isDragging = true;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _cast.blocksRaycasts = true;
        _shadowTween = Tween.LocalPosition(_shadow, _shadowPosition, _fadeTime);
        _holder?.OnEndDragElement(eventData.position);
        _isDragging = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        _transform.position = eventData.position;
        _shadow.position = new(_transform.position.x, _transform.position.y - _height);
        _holder?.OnDragElement(_transform.position);
    }
}