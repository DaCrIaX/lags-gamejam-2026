namespace UnityEngine.EventSystems
{
    using Audio;
    using Events;

    public abstract class HOVCardsGroup : MonoBehaviour
    {
        [SerializeReference] private Transform _container;
        [SerializeReference] private AudioEmitter _audioSwipe;
        [SerializeField] private int _maxAmount;

        [SerializeField] private LayoutHandler _position, _rotation;
        [SerializeField] private UnityEvent<bool> _isNotEmpty;
        [SerializeField] private UnityEvent<bool> _isFull;

        protected GameplayManager _gameplay;
        protected CardTransform[] _cards;

        protected float _inverseLength;

        public Transform Container => _container;
        public int MaxAmount { get => _maxAmount; set => _maxAmount = value; }
        public int Amount => _cards != null ? _cards.Length : 0;
        public bool HasAvailableSpace => Amount < _maxAmount;

        protected virtual void Awake() => _gameplay = GameplayManager.Instance;
        protected virtual void Start() => RefreshCardsArray();
        protected virtual void OnTransformChildrenChanged() => RefreshCardsArray();

        public async void ClearChildren()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
                Destroy(_container.GetChild(i).gameObject);

            await Awaitable.NextFrameAsync();
            RefreshCardsArray();
        }
        private void RefreshCardsArray()
        {
            _cards = GetComponentsInChildren<CardTransform>();
            _isNotEmpty.Invoke(_cards.Length != 0);
            _isFull.Invoke(_cards.Length == _maxAmount);
            _inverseLength = 1f / _cards.Length;
            _inverseLength += _inverseLength * 0.5f;
        }

        public float GetPosition(int index) => GetAnimation(ref _position, index);
        public float GetRotation(int index) => GetAnimation(ref _rotation, index);
        private float GetAnimation(ref LayoutHandler layout, int index) =>
            layout.Evaluate(Amount <= 1 ? 0.5f : index * _inverseLength);

        public abstract void OnBeginDrag(CardTransform card);
        public abstract void OnDragElement(Vector2 position);
        public abstract void OnDropElement(Vector2 position);
        protected virtual void OnSwipe(int index)
        {
            _gameplay.Selected.SiblingIndex = index;
            _audioSwipe?.PlayOneShot();
        }

        public bool DropItem(CardTransform card)
        {
            if (!HasAvailableSpace) return false;
            card.SetParent(Container);
            return true;
        }

        public void AllCardsBackToHand()
        {
            _cards = GetComponentsInChildren<CardTransform>();
            foreach (var card in _cards) {
                card.GetComponentInChildren<CardSelectable>().BackToHand();
            }
        }
    }
}