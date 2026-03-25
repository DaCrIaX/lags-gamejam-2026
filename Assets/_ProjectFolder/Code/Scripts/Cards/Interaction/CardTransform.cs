namespace UnityEngine.EventSystems
{
    public class CardTransform : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform, _cardTransform;
        [SerializeField] private float _gridSnapSpeed = 10f;

        private GameplayManager _manager;
        private HOVCardsGroup _cardGroup;
        private CanvasGroup _group;

        public GameplayManager Manager => _manager;
        public HOVCardsGroup CardGroup => _cardGroup;
        public RectTransform RectTransform => _transform;
        public RectTransform CardRectTransform => _cardTransform;

        public int SiblingIndex
        {
            get => _transform.GetSiblingIndex();
            set => _transform.SetSiblingIndex(value);
        }

        public Vector2 Position => _transform.position;

        public bool IsDragging { get; set; }
        public bool IsHovering { get; set; }

        private void Awake()
        {
            _manager = GameplayManager.Instance;
            _group = GetComponentInChildren<CanvasGroup>();
        }
        private void Start() => OnTransformParentChanged();
        private void OnEnable() => _manager.onObjectSelectedChanged += OnObjectSelectedChanged;
        private void OnDisable() => _manager.onObjectSelectedChanged -= OnObjectSelectedChanged;
        private void OnObjectSelectedChanged(bool value) => _group.blocksRaycasts = value;

        private void LateUpdate()
        {
            if (!CardGroup || IsDragging) return;

            int index = SiblingIndex;
            float speed = _gridSnapSpeed * Time.deltaTime;

            var targetLocalPos = new Vector3(0, _cardGroup.GetPosition(index), 0);
            var targetRot = Quaternion.Euler(0, 0, -_cardGroup.GetRotation(index));

            _cardTransform.localPosition = Vector3.Lerp(_cardTransform.localPosition, targetLocalPos, speed);
            _cardTransform.localRotation = Quaternion.Lerp(_cardTransform.localRotation, targetRot, speed);
        }
        private void OnTransformParentChanged()
        {
            var group = GetComponentInParent<HOVCardsGroup>();
            if (group)
                _cardGroup = group;
        }

        public void SetParent(Transform parent) => _transform.SetParent(parent);
        public void SetCardParent(Transform parent) => _cardTransform.SetParent(parent);
        public void ResetCardParent() => _cardTransform.SetParent(_transform);
    }
}