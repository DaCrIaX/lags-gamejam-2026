namespace UnityEngine.EventSystems
{
    [RequireComponent(typeof(Card))]
    public abstract class CardBehaviour : MonoBehaviour
    {
        protected Canvas _canvas;
        protected Camera _camera;
        protected Card _card;

        protected Vector2 CardSize => _card.CardTransform.rect.size;
        protected Vector2 Bounds => 0.5f * CardSize;

        protected virtual void Awake()
        {
            _camera = Camera.main;
            _canvas = GetComponentInParent<Canvas>();
            _card = GetComponent<Card>();
        }
    }
}