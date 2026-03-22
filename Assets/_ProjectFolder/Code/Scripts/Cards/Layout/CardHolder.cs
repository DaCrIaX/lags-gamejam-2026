namespace UnityEngine.EventSystems
{
    public abstract class CardHolder : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private int _maxAmount;
        [SerializeField] private LayoutHandler _position, _rotation;

        protected GameplayManager _gameplay;
        protected Card[] _cards;

        protected float _inverseLength;

        public Transform Container => _container;
        public int MaxAmount => _maxAmount;
        public bool CanAddElement => _cards.Length < _maxAmount;

        protected virtual void Awake() => _gameplay = GameplayManager.Instance;
        protected virtual void Start() => RefreshCardsArray();

        public void RefreshCardsArray()
        {
            _cards = GetComponentsInChildren<Card>();
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

        public abstract void OnBeginDrag(Card card);
        public abstract void OnDragElement(Vector2 position);
        public abstract void OnDropElement(Vector2 position);
    }
}