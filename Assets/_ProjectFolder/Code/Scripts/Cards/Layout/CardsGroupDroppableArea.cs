using UnityEngine.Audio;

namespace UnityEngine.EventSystems
{
    public class CardsGroupDroppableArea : MonoBehaviour, IDropHandler
    {
        [SerializeField] private AudioEmitter _dropAudio;
        private HOVCardsGroup _connected;

        private void Awake() => _connected = GetComponentInChildren<HOVCardsGroup>();

        public void OnDrop(PointerEventData eventData)
        {
            if (!_connected.HasAvailableSpace) return;
            if (!eventData.pointerDrag.TryGetComponent(out CardDrag card)) return;
            
            card.CardTransform.SetParent(_connected.Container);
            _dropAudio.PlayOneShot();
        }
    }
}