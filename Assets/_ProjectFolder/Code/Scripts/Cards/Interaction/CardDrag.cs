namespace UnityEngine.EventSystems
{
    using Animations;
    using InputSystem;

    public class CardDrag : CardBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float _smoothSpeed = 10f;
        [SerializeField] private float _inputDeltaMultiply = 5f;

        [Header("VFX")]
        [SerializeField] private TweenRectPositionSwipe _cardShadow;

        private Vector3 _positionTarget, _rotationDelta;

        private void LateUpdate()
        {
            if (!_cardTransform.IsDragging) return;

            int index = _cardTransform.SiblingIndex;
            float speed = Time.deltaTime * _smoothSpeed;

            Vector2 movementRotation = Mouse.current.delta.value * _inputDeltaMultiply;
            _rotationDelta = Vector3.Lerp(_rotationDelta, movementRotation, speed);

            _cardTransform.CardRectTransform.position = Vector2.Lerp(_cardTransform.CardRectTransform.position, _positionTarget, speed);
            _cardTransform.CardRectTransform.eulerAngles = new(0, 0, Mathf.Clamp(_rotationDelta.x, -60, 60));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _cardTransform.CardGroup?.OnBeginDrag(_cardTransform);
            _cardTransform.IsDragging = true;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _cardTransform.CardGroup?.OnDropElement(eventData.pointerCurrentRaycast.worldPosition);
            _cardTransform.IsDragging = false;
            _cardShadow?.ForceSwipeIn();
        }
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 bounds = Bounds, point = eventData.position;
            point.x = Mathf.Clamp(point.x, bounds.x, Screen.width - bounds.x);
            point.y = Mathf.Clamp(point.y, bounds.y, Screen.height - bounds.y);

            _positionTarget = _cardTransform.Manager.ScreenToWorldPoint(point);
            _cardShadow.SetPosition(_cardTransform.CardRectTransform.position);
            _cardTransform.CardGroup?.OnDragElement(_positionTarget);
        }
    }
}