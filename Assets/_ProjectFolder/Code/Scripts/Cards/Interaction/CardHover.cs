using PrimeTween;

namespace UnityEngine.EventSystems
{
    using Animations;

    public class CardHover : CardBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerMoveHandler
    {
        [SerializeField] private TweenScale _cardSize;

        private Tween _reset;
        private Vector2 _input;

        private void LateUpdate()
        {
            if (_cardTransform.IsDragging || !_cardTransform.IsHovering) return;

            float time = Time.time * _gameplay.Frequency;
            float sine = Mathf.Sin(time) * _gameplay.Amplitude;
            float cosine = Mathf.Cos(time) * _gameplay.Amplitude;

            Vector2 offset = (Vector2)_cardTransform.RectTransform.position - _input;
            float tiltX = -offset.y * _gameplay.Offset;
            float tiltY = offset.x * _gameplay.Offset;

            float speed = _gameplay.DeltaSpeed * Time.deltaTime;
            float lerpX = Mathf.LerpAngle(_cardTransform.RectTransform.eulerAngles.x, tiltX + sine, speed);
            float lerpY = Mathf.LerpAngle(_cardTransform.RectTransform.eulerAngles.y, tiltY + cosine, speed);
            _cardTransform.RectTransform.eulerAngles = new Vector3(lerpX, lerpY, 0f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _reset.Complete();
            _cardSize.ScaleIn();
            _cardTransform.IsHovering = true;
            _input = eventData.pointerCurrentRaycast.worldPosition;
            _cardTransform.RectTransform.LocalPositionZ(1);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _cardSize.ScaleOut();
            _cardTransform.IsHovering = false;
            _cardTransform.RectTransform.LocalPositionZ(0);
            _reset = Tween.Rotation(_cardTransform.RectTransform, Quaternion.identity, 0.2f);
        }

        public void OnPointerDown(PointerEventData eventData) => _cardSize.ScaleOut();
        public void OnPointerMove(PointerEventData eventData) => _input = eventData.pointerCurrentRaycast.worldPosition;
    }
}