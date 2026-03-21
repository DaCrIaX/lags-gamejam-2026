namespace UnityEngine.UI
{
    public abstract class ScrollVirtualizedHorizontalGrid<T> : ScrollVirtualizedHorizontal<T>
    {
        [SerializeField] protected int _rows = 3;
        protected float _itemHeight;

        protected override void Awake()
        {
            base.Awake();
            _itemHeight = _prefab.RectTransform.rect.height;
        }
        protected override void ComputeVisibleRange(float scrollPos, float viewportSize, out int start, out int end)
        {
            int firstCol = _layoutManager.GetIndexAtPosition(scrollPos);
            int lastCol = _layoutManager.GetIndexAtPosition(scrollPos + viewportSize);

            start = Mathf.Max(0, firstCol * _rows);
            end = Mathf.Min((lastCol + 1) * _rows - 1, _data.Count - 1);
        }
        protected override void SetItemLayout(RectTransform rt, int index)
        {
            int col = index / _rows;
            int row = index % _rows;

            float posX = _layoutManager.GetPosition(col);
            float posY = row * (_itemHeight + _spacing);

            rt.anchorMin = new(0, 1);
            rt.anchorMax = new(0, 1);
            rt.pivot = new(0, 1);

            rt.anchoredPosition = new(posX, -posY);
            rt.sizeDelta = new(_layoutManager.GetSize(col), _itemHeight);
        }
        protected override void CalculateLayout()
        {
            int totalColumns = Mathf.CeilToInt((float)_data.Count / _rows);
            _layoutManager.CalculateLayout(totalColumns, GetDefaultItemSize(), _spacing, GetColumnWidth);
            UpdateContainerSize(_layoutManager.TotalSize);
        }

        protected virtual float GetColumnWidth(int colIndex) => GetDefaultItemSize();
    }
}