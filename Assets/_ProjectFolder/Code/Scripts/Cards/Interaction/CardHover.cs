using PrimeTween;

namespace UnityEngine.EventSystems
{
    using Animations;

    [RequireComponent(typeof(CardTransform))]
    public class CardHover : CardBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerMoveHandler
    {
        [SerializeField] private float _waveAmplitude = 1f, _waveFrequency = 1f;
        [SerializeField] private float _smoothSpeed, _offsetMultiply;

        [Header("VFX")]
        [SerializeField] private TweenScale _cardSize;

        private Tween _reset;
        private Vector2 _input;

        private void LateUpdate()
        {
            if (_card.IsDragging || !_card.IsHovering) return;

            float time = Time.time * _waveFrequency;
            float sine = Mathf.Sin(time) * _waveAmplitude;
            float cosine = Mathf.Cos(time) * _waveAmplitude;

            Vector2 offset = (Vector2)_card.RectTransform.position - _input;
            float tiltX = -offset.y * _offsetMultiply;
            float tiltY = offset.x * _offsetMultiply;

            float speed = _smoothSpeed * Time.deltaTime;
            float lerpX = Mathf.LerpAngle(_card.RectTransform.eulerAngles.x, tiltX + sine, speed);
            float lerpY = Mathf.LerpAngle(_card.RectTransform.eulerAngles.y, tiltY + cosine, speed);
            _card.RectTransform.eulerAngles = new Vector3(lerpX, lerpY, 0f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _reset.Complete();
            _cardSize.ScaleIn();
            _card.IsHovering = true;
            _input = eventData.pointerCurrentRaycast.worldPosition;
            _card.RectTransform.LocalPositionZ(1);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _cardSize.ScaleOut();
            _card.IsHovering = false;
            _card.RectTransform.LocalPositionZ(0);
            _reset = Tween.Rotation(_card.RectTransform, Quaternion.identity, 0.2f);
        }

        public void OnPointerDown(PointerEventData eventData) => _cardSize.ScaleOut();
        public void OnPointerMove(PointerEventData eventData) => _input = eventData.pointerCurrentRaycast.worldPosition;
    }
}