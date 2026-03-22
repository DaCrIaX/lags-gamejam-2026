namespace UnityEngine.EventSystems
{
    [RequireComponent(typeof(CardGroup))]
    public class CardGroupDroppable : MonoBehaviour, IDropHandler
    {
        private CardGroup _holder;

        private void Awake() => _holder = GetComponent<CardGroup>();

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent(out CardTransform card))
                card.CardGroup.DropItem(card, _holder);
        }
    }
}