namespace UnityEngine.EventSystems
{
    [RequireComponent(typeof(CardTransform))]
    public abstract class CardBehaviour : MonoBehaviour
    {
        protected Canvas _canvas;
        protected Camera _camera;
        protected CardTransform _card;

        protected Vector2 CardSize => _card.CardRectTransform.rect.size;
        protected Vector2 Bounds => 0.5f * CardSize;

        protected virtual void Awake()
        {
            _camera = Camera.main;
            _canvas = GetComponentInParent<Canvas>();
            _card = GetComponent<CardTransform>();
        }
    }
}