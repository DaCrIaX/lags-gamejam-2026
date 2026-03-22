namespace UnityEngine.EventSystems
{
    [RequireComponent(typeof(CardTransform))]
    public abstract class CardBehaviour : MonoBehaviour
    {
        protected CardTransform _card;

        protected Vector2 CardSize => _card.CardRectTransform.rect.size;
        protected Vector2 Bounds => 0.5f * CardSize;

        protected virtual void Awake() => _card = GetComponent<CardTransform>();
    }
}