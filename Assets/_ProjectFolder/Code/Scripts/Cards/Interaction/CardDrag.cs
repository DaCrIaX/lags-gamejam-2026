namespace UnityEngine.EventSystems
{
    using Animations;
    using InputSystem;

    [RequireComponent(typeof(CardTransform))]
    public class CardDrag : CardBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Vector2 _cardSize;
        [SerializeField] private float _smoothSpeed = 10f;
        [SerializeField] private float _deltaMultiply = 5f;

        [Header("VFX")]
        [SerializeField] private TweenRectPositionSwipe _cardShadow;

        private Vector3 _positionTarget;
        private Vector3 _rotationDelta;

        private void LateUpdate()
        {
            if (!_card.Parent) return;

            int index = _card.SiblingIndex;
            float speed = Time.deltaTime * _smoothSpeed;

            if (!_card.IsDragging)
            {
                var targetLocalPos = new Vector3(0, _card.Parent.GetPosition(index), 0);
                var targetRot = Quaternion.Euler(0, 0, -_card.Parent.GetRotation(index));

                _card.CardRectTransform.localPosition = Vector3.Lerp(_card.CardRectTransform.localPosition, targetLocalPos, speed);
                _card.CardRectTransform.localRotation = Quaternion.Lerp(_card.CardRectTransform.localRotation, targetRot, speed);
            }
            else
            {
                Vector2 movementRotation = Mouse.current.delta.value * _deltaMultiply;
                _rotationDelta = Vector3.Lerp(_rotationDelta, movementRotation, speed);

                _card.CardRectTransform.position = Vector2.Lerp(_card.CardRectTransform.position, _positionTarget, speed);
                _card.CardRectTransform.eulerAngles = new(0, 0, Mathf.Clamp(_rotationDelta.x, -60, 60));
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _card.Parent?.OnBeginDrag(_card);
            _card.IsDragging = true;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _card.Parent?.OnDropElement(eventData.pointerCurrentRaycast.worldPosition);
            _card.IsDragging = false;
            _cardShadow?.ForceSwipeIn();
        }
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 bounds = Bounds;
            Vector2 point = eventData.position;
            point.x = Mathf.Clamp(point.x, bounds.x, Screen.width - bounds.x);
            point.y = Mathf.Clamp(point.y, bounds.y, Screen.height - bounds.y);

            _positionTarget = _camera.ScreenToWorldPoint(new Vector3(point.x, point.y, _canvas.planeDistance));
            _cardShadow.SetPosition(_card.CardRectTransform.position);
            _card.Parent?.OnDragElement(_positionTarget);
        }
    }
}