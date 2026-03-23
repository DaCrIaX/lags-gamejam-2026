namespace UnityEngine.EventSystems
{
    public class CardTransform : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform, _cardTransform;
        [SerializeField] private float _snapSpeed = 1f;

        private GameplayManager _manager;
        private CardGroup _cardGroup;
        private CanvasGroup _group;

        public GameplayManager Manager => _manager;
        public CardGroup CardGroup => _cardGroup;
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
            _group = GetComponent<CanvasGroup>();
        }
        private void Start() => SearchParentGroup();
        private void OnEnable() => _manager.onObjectSelectedChanged += ChangeBlockRaycast;
        private void OnDisable() => _manager.onObjectSelectedChanged -= ChangeBlockRaycast;

        private void LateUpdate()
        {
            if (!CardGroup || IsDragging) return;

            int index = SiblingIndex;
            float speed = _snapSpeed * Time.deltaTime;

            var targetLocalPos = new Vector3(0, _cardGroup.GetPosition(index), 0);
            var targetRot = Quaternion.Euler(0, 0, -_cardGroup.GetRotation(index));

            _cardTransform.localPosition = Vector3.Lerp(_cardTransform.localPosition, targetLocalPos, speed);
            _cardTransform.localRotation = Quaternion.Lerp(_cardTransform.localRotation, targetRot, speed);
        }

        public void SearchParentGroup() => _cardGroup = GetComponentInParent<CardGroup>();
        public void RefreshParent() => _cardGroup.RefreshCardsArray();
        public void ChangeBlockRaycast(bool value) => _group.blocksRaycasts = value;

        public void ResetCardParent() => _cardTransform.SetParent(_transform);
        public void SetCardParent(Transform parent) => _cardTransform.SetParent(parent);
        public void SetParent(Transform parent) => _transform.SetParent(parent);
    }
}