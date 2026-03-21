namespace UnityEngine.UI
{
    public abstract class ScrollVirtualizedHorizontal<T> : ScrollVirtualizedBase<T>
    {
        protected override void SetItemLayout(RectTransform rt, int index)
        {
            float pos = _layoutManager.GetPosition(index);
            float size = _layoutManager.GetSize(index);

            rt.anchorMin = new(0, 0);
            rt.anchorMax = new(0, 1);
            rt.pivot = new(0, 0.5f);

            rt.anchoredPosition = new(pos, 0);
            rt.sizeDelta = new(size, 0);
        }
        protected override void UpdateContainerSize(float totalSize)
        {
            _scroll.content.sizeDelta = new(totalSize, _scroll.content.sizeDelta.y);
        }

        protected override float GetDefaultItemSize() => _prefab.RectTransform.rect.width;
        protected override float GetScrollPosition() => -_scroll.content.anchoredPosition.x;
        protected override float GetViewportSize() => _scroll.viewport.rect.width;
        protected override float GetDynamicSize(int index) => GetDefaultItemSize();
    }
}