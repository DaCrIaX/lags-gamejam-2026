namespace UnityEngine.EventSystems
{
    public class CardHolderDroppable : MonoBehaviour, IDropHandler
    {
        [SerializeField] private CardHolder _holder;

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