namespace UnityEngine.EventSystems
{
    public class CardsGroupDroppableArea : MonoBehaviour, IDropHandler
    {
        private HOVCardsGroup _connected;

        private void Awake() => _connected = GetComponentInChildren<HOVCardsGroup>();

        public void OnDrop(PointerEventData eventData)
        {
            if (!_connected.HasAvailableSpace) return;

            if (eventData.pointerDrag.TryGetComponent(out CardDrag card))
                card.CardTransform.SetParent(_connected.Container);
        }
    }
}