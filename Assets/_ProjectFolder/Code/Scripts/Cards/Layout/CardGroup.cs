namespace UnityEngine.EventSystems
{
    using Audio;

    public abstract class CardGroup : MonoBehaviour
    {
        [SerializeField] private CardGroup _connected;
        [SerializeField] private Transform _container;
        [SerializeField] private int _maxAmount;

        [SerializeField] private LayoutHandler _position, _rotation;
        [SerializeReference] protected AudioEmitter _audioSwipe, _audioDrop;

        protected GameplayManager _gameplay;
        protected CardTransform[] _cards;

        protected float _inverseLength;

        public Transform Container => _container;
        public int MaxAmount => _maxAmount;
        public bool CanAddElement => _cards.Length < _maxAmount;

        protected virtual void Awake() => _gameplay = GameplayManager.Instance;
        protected virtual void Start() => RefreshCardsArray();

        public void RefreshCardsArray()
        {
            _cards = GetComponentsInChildren<CardTransform>();
            _inverseLength = 1f / _cards.Length;
            _inverseLength += _inverseLength * 0.5f;
        }
        public void ClearChildren()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
                Destroy(_container.GetChild(i).gameObject);
        }

        public float GetPosition(int index) => GetAnimation(ref _position, index);
        public float GetRotation(int index) => GetAnimation(ref _rotation, index);
        private float GetAnimation(ref LayoutHandler animation, int index) =>
            animation.Evaluate(_cards.Length <= 1 ? 0.5f : index * _inverseLength);

        public abstract void OnBeginDrag(CardTransform card);
        public abstract void OnDragElement(Vector2 position);
        public abstract void OnDropElement(Vector2 position);

        public void DropItemToConnection(CardTransform card)
        {
            if (_connected)
                DropItem(card, _connected);
        }
        public async void DropItem(CardTransform card, CardGroup holder)
        {
            if (!holder.CanAddElement) return;
            card.SetParent(holder.Container);
            _audioDrop?.PlayOneShot();
            
            await Awaitable.EndOfFrameAsync();
            card.RefreshParent();
            card.SearchParentGroup();
            holder.RefreshCardsArray();
        }
    }
}