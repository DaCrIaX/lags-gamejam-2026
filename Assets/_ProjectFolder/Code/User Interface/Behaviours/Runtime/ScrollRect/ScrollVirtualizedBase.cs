using System.Collections.Generic;

namespace UnityEngine.UI
{
    using Pool;

    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollVirtualizedBase<T> : MonoBehaviour
    {
        [SerializeField] protected ScrollVirtualObject _prefab;
        [SerializeField] protected float _spacing;
        [SerializeField] private int _buffer = 2;

        protected VirtualLayoutHandler _layoutManager = new();
        protected ScrollRect _scroll;
        protected IList<T> _data;

        protected readonly Dictionary<int, ScrollVirtualObject> _activeItems = new();
        private int _firstVisible = -1, _lastVisible = -1;

        protected virtual void Awake()
        {
            _scroll = GetComponent<ScrollRect>();
            _scroll.onValueChanged.AddListener(_ => OnUpdateScroll());
            _pool = new(OnCreate, OnGet, OnRelease, OnDestroyObject);
        }

        #region Object Pooling Logic
        protected ObjectPool<ScrollVirtualObject> _pool;

        protected virtual ScrollVirtualObject OnCreate()
        {
            var item = Instantiate(_prefab, _scroll.content);
            item.PoolReference = _pool;
            return item;
        }
        protected virtual void OnGet(ScrollVirtualObject @object) => @object.Enable();
        protected virtual void OnRelease(ScrollVirtualObject @object) => @object.Disable();
        protected virtual void OnDestroyObject(ScrollVirtualObject @object) => Destroy(@object.gameObject);
        protected void Clear()
        {
            foreach (var item in _activeItems.Values)
                item.Destroy();

            _activeItems.Clear();
        }
        #endregion

        public void SetData(IList<T> values)
        {
            _data = values;
            Canvas.ForceUpdateCanvases();

            CalculateLayout();
            ResetView();
            OnUpdateScroll(true);
        }
        private void ResetView()
        {
            _scroll.content.anchoredPosition = Vector2.zero;
            _firstVisible = _lastVisible = -1;
            Clear();
        }

        private void OnUpdateScroll(bool force = false)
        {
            if (_data == null || _data.Count == 0) return;

            float scrollPos = Mathf.Abs(GetScrollPosition());
            float viewportSize = GetViewportSize();

            ComputeVisibleRange(scrollPos, viewportSize, out int newFirst, out int newLast);

            if (force || newFirst != _firstVisible || newLast != _lastVisible)
            {
                _firstVisible = newFirst;
                _lastVisible = newLast;
                RefreshLayout();
            }
        }
        private void RefreshLayout()
        {
            CleanOutOfBounds();
            UpdateVisibleItems();
        }
        private void CleanOutOfBounds()
        {
            var activeKeys = new List<int>(_activeItems.Keys);
            foreach (var index in activeKeys)
            {
                if (index < _firstVisible || index > _lastVisible)
                {
                    _activeItems[index].Destroy();
                    _activeItems.Remove(index);
                }
            }
        }
        private void UpdateVisibleItems()
        {
            for (int i = _firstVisible; i <= _lastVisible; i++)
            {
                if (!_activeItems.TryGetValue(i, out ScrollVirtualObject item))
                {
                    item = _pool.Get();
                    Bind(item, i);
                    _activeItems.Add(i, item);
                }

                SetItemLayout(item.RectTransform, i);
            }
        }

        protected virtual void ComputeVisibleRange(float scrollPos, float viewportSize, out int start, out int end)
        {
            start = Mathf.Max(0, _layoutManager.GetIndexAtPosition(scrollPos) - _buffer);
            end = Mathf.Min(_data.Count - 1, _layoutManager.GetIndexAtPosition(scrollPos + viewportSize) + _buffer);
        }

        protected abstract void UpdateContainerSize(float totalSize);
        protected abstract void SetItemLayout(RectTransform rt, int index);
        protected abstract void Bind(ScrollVirtualObject item, int dataIndex);
        protected abstract void CalculateLayout();

        protected abstract float GetDefaultItemSize();
        protected abstract float GetDynamicSize(int index);
        protected abstract float GetScrollPosition();
        protected abstract float GetViewportSize();
    }
}