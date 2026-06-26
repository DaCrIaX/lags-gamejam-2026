namespace UnityEngine.EventSystems
{
    public class CardSelectable : CardBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _timeClicked;
        private float _currentTime;

        public void OnPointerDown(PointerEventData eventData) => _currentTime = Time.time;
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_card.Ingredient || Time.time - _currentTime > _timeClicked) return;

            var navigation = GetComponentInParent<CardsGroupNavigation>();
            if (!navigation) return;

            navigation.SetCardDirection(_cardTransform, _card.Ingredient.CardType);
        }

        public void BackToHand()
        {
            var navigation = GetComponentInParent<CardsGroupNavigation>();
            if (!navigation) return;

            navigation.SetCardDirection(_cardTransform, _card.Ingredient.CardType);
        }
    }
}