using UnityEngine;
using UnityEngine.EventSystems;

public class CardPlaceHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] private CardHolderHorizontal _holder;

    public async void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.TryGetComponent(out Card card)) return;

        card.SetParent(_holder.transform);
        await Awaitable.EndOfFrameAsync();
        card.RefreshCardHolder();
        _holder.RefreshCardsArray();
    }
}