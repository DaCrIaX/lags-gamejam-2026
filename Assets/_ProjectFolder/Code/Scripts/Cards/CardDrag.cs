namespace UnityEngine.EventSystems
{
    using Animations;
    using InputSystem;

    [RequireComponent(typeof(Card))]
    public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TweenRectPositionSwipe _shadow;
        [SerializeField] private float _smoothSpeed = 10f;
        [SerializeField] private float _deltaMultiply = 5f;

        private Card _card;
        private Vector2 _positionTarget;
        private Vector3 _rotationDelta;

        private void Awake() => _card = GetComponent<Card>();
        private void LateUpdate()
        {
            if (!_card.Parent) return;

            int index = _card.SiblingIndex;
            float speed = Time.deltaTime * _smoothSpeed;

            if (!_card.IsDragging)
            {
                var targetLocalPos = new Vector3(0, _card.Parent.GetPosition(index), 0);
                var targetRot = Quaternion.Euler(0, 0, -_card.Parent.GetRotation(index));

                _card.CardTransform.localPosition = Vector3.Lerp(_card.CardTransform.localPosition, targetLocalPos, speed);
                _card.CardTransform.localRotation = Quaternion.Lerp(_card.CardTransform.localRotation, targetRot, speed);
            }
            else
            {
                Vector2 movementRotation = Mouse.current.delta.value * _deltaMultiply;
                _rotationDelta = Vector3.Lerp(_rotationDelta, movementRotation, speed);

                _card.CardTransform.position = Vector2.Lerp(_card.CardTransform.position, _positionTarget, speed);
                _card.CardTransform.eulerAngles = new(_card.CardTransform.eulerAngles.x, _card.CardTransform.eulerAngles.y, Mathf.Clamp(_rotationDelta.x, -60, 60));
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _card.Parent?.OnBeginDrag(_card);
            _card.IsDragging = true;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _card.Parent?.OnDropElement(eventData.position);
            _card.IsDragging = false;
            _shadow?.ForceSwipeIn();
        }
        public void OnDrag(PointerEventData eventData)
        {
            _shadow.SetPosition(_card.CardTransform.position);
            _positionTarget = eventData.position;
            _card.Parent?.OnDragElement(eventData.position);
        }
    }
}