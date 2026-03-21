namespace UnityEngine.UI
{
    public abstract class ScrollVirtualizedVerticalGrid<T> : ScrollVirtualizedVertical<T>
    {
        [SerializeField] protected int _columns = 3;
        protected float _itemWidth;

        protected override void Awake()
        {
            base.Awake();
            _itemWidth = _prefab.RectTransform.rect.width;
        }
        protected override void ComputeVisibleRange(float scrollPos, float viewportSize, out int start, out int end)
        {
            int firstRow = _layoutManager.GetIndexAtPosition(scrollPos);
            int lastRow = _layoutManager.GetIndexAtPosition(scrollPos + viewportSize);

            start = Mathf.Max(0, firstRow * _columns);
            end = Mathf.Min((lastRow + 1) * _columns - 1, _data.Count - 1);
        }
        protected override void SetItemLayout(RectTransform rt, int index)
        {
            int row = index / _columns;
            int col = index % _columns;

            float posY = _layoutManager.GetPosition(row);
            float posX = col * (_itemWidth + _spacing);

            rt.anchorMin = new(0, 1);
            rt.anchorMax = new(0, 1);
            rt.pivot = new(0, 1);

            rt.anchoredPosition = new(posX, -posY);
            rt.sizeDelta = new(_itemWidth, _layoutManager.GetSize(row));
        }
        protected override void CalculateLayout()
        {
            int totalRows = Mathf.CeilToInt((float)_data.Count / _columns);
            _layoutManager.CalculateLayout(totalRows, GetDefaultItemSize(), _spacing, GetRowHeight);
            UpdateContainerSize(_layoutManager.TotalSize);
        }

        protected virtual float GetRowHeight(int rowIndex) => GetDefaultItemSize();
    }
}