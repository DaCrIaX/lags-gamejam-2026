namespace UnityEngine.EventSystems
{
    [RequireComponent(typeof(CardHolder))]
    public class CardHolderDroppable : MonoBehaviour, IDropHandler
    {
        private CardHolder _holder;

        private void Awake() => _holder = GetComponent<CardHolder>();

        public async void OnDrop(PointerEventData eventData)
        {
            if (!_holder.CanAddElement) return;
            if (!eventData.pointerDrag.TryGetComponent(out Card card)) return;

            card.SetParent(_holder.Container);
            await Awaitable.EndOfFrameAsync();

            card.RefreshParent();
            card.SearchParentGroup();
            _holder.RefreshCardsArray();
        }
    }
}