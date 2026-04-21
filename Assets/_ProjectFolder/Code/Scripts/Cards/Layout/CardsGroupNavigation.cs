using System;
using UnityEngine.Audio;

namespace UnityEngine.EventSystems
{
    public class CardsGroupNavigation : MonoBehaviour
    {
        [Serializable] private struct Direction
        {
            public string name;
            public CardType cardType;
            public HOVCardsGroup group;
        }

        [SerializeField] private AudioEmitter _onNavigateSound;
        [SerializeField] private Direction[] _directions;

        public void SetCardDirection(CardTransform card, CardType type)
        {
            foreach (Direction direction in _directions)
            {
                if (!direction.group || !direction.cardType.HasFlag(type)) continue;
                if (!direction.group.gameObject.activeSelf) continue;
                if (!direction.group.HasAvailableSpace) return;

                card.SetParent(direction.group.Container);
                _onNavigateSound.PlayOneShot();
                return;
            }
        }
    }
}