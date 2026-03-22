namespace UnityEngine.EventSystems
{
    public class CardGroupHorizontal : CardGroup
    {
        public override void OnBeginDrag(CardTransform card)
        {
            _gameplay.SelectCard(card);
            _gameplay.Selected.SetCardParent(_gameplay.DragArea);
        }
        public override void OnDragElement(Vector2 position)
        {
            for (int i = 0; i < _cards.Length; i++)
            {
                if (_cards[i] == _gameplay.Selected) continue;

                float siblingX = _cards[i].Position.x;
                int selectedIdx = _gameplay.Selected.SiblingIndex;
                int siblingIdx = _cards[i].SiblingIndex;

                if (position.x > siblingX && selectedIdx < siblingIdx) Swipe(siblingIdx);
                else if (position.x < siblingX && selectedIdx > siblingIdx) Swipe(siblingIdx);
            }
        }
        public override void OnDropElement(Vector2 position)
        {
            _gameplay.Selected.ResetCardParent();
            _gameplay.SelectCard(null);
        }

        private void Swipe(int index)
        {
            _gameplay.Selected.SiblingIndex = index;
            _audioSwipe?.PlayOneShot();
        }
    }
}