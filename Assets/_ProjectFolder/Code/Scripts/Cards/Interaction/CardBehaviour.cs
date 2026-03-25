namespace UnityEngine.EventSystems
{
    public abstract class CardBehaviour : MonoBehaviour
    {
        protected Card _card;
        protected CardTransform _cardTransform;

        public CardTransform CardTransform => _cardTransform;

        protected Vector2 CardSize => _cardTransform.CardRectTransform.rect.size;
        protected Vector2 Bounds => 0.5f * CardSize;

        protected virtual void Awake()
        {
            _card = GetComponentInParent<Card>();
            _cardTransform = GetComponentInParent<CardTransform>();
        }
    }
}