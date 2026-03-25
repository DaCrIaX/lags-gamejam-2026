namespace UnityEngine.EventSystems
{
    public class HorizontalCardsGroup : HOVCardsGroup
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
                int siblingIdx = _cards[i].SiblingIndex;
                int selectedIdx = _gameplay.Selected.SiblingIndex;

                if (position.x > siblingX && selectedIdx < siblingIdx) OnSwipe(siblingIdx);
                else if (position.x < siblingX && selectedIdx > siblingIdx) OnSwipe(siblingIdx);
            }
        }
        public override void OnDropElement(Vector2 position)
        {
            _gameplay.Selected.ResetCardParent();
            _gameplay.SelectCard(null);
        }
    }
}