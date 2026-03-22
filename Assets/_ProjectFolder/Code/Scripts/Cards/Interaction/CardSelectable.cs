namespace UnityEngine.EventSystems
{
    public class CardSelectable : CardBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _timeClicked;
        private float _currentTime;

        public void OnPointerDown(PointerEventData eventData) => _currentTime = Time.time;
        public void OnPointerUp(PointerEventData eventData)
        {
            if (Time.time - _currentTime < _timeClicked)
                _card.CardGroup.DropItemToConnection(_card);
        }
    }
}