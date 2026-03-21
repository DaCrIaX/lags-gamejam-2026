namespace UnityEngine.UI
{
    public abstract class ScrollVirtualizedVertical<T> : ScrollVirtualizedBase<T>
    {
        protected override void SetItemLayout(RectTransform rt, int index)
        {
            float pos = _layoutManager.GetPosition(index);
            float size = _layoutManager.GetSize(index);

            rt.anchorMin = new(0, 1);
            rt.anchorMax = new(1, 1);
            rt.pivot = new(0.5f, 1);

            rt.anchoredPosition = new(0, -pos);
            rt.sizeDelta = new(0, size);
        }
        protected override void UpdateContainerSize(float totalSize)
        {
            _scroll.content.sizeDelta = new(_scroll.content.sizeDelta.x, totalSize);
        }
        protected override void CalculateLayout()
        {
            _layoutManager.CalculateLayout(_data.Count, GetDefaultItemSize(), _spacing, GetDynamicSize);
            UpdateContainerSize(_layoutManager.TotalSize);
        }

        protected override float GetDefaultItemSize() => _prefab.RectTransform.rect.height;
        protected override float GetScrollPosition() => _scroll.content.anchoredPosition.y;
        protected override float GetViewportSize() => _scroll.viewport.rect.height;
        protected override float GetDynamicSize(int index) => GetDefaultItemSize();
    }
}