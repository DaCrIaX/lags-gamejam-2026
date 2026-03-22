using UnityEngine.Animations;

namespace UnityEngine.EventSystems
{
    [RequireComponent(typeof(Card))]
    public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private TweenScale _cardSize;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _cardSize.ScaleIn();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _cardSize.ScaleOut();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _cardSize.ScaleOut();
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}