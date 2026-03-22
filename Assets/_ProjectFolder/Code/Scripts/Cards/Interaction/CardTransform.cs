namespace UnityEngine.EventSystems
{
    public class CardTransform : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform, _cardTransform;

        private CanvasGroup _group;
        private CardHolder _parent;

        public CardHolder Parent => _parent;
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

        private void Awake() => _group = GetComponent<CanvasGroup>();
        private void Start() => _parent = GetComponentInParent<CardHolder>();
        private void OnEnable() => GameplayManager.Instance.onObjectSelectedChanged += ChangeBlockRaycast;
        private void OnDisable() => GameplayManager.Instance.onObjectSelectedChanged -= ChangeBlockRaycast;

        public void SearchParentGroup() => Start();
        public void RefreshParent() => _parent.RefreshCardsArray();
        public void ChangeBlockRaycast(bool value) => _group.blocksRaycasts = value;

        public void ResetCardParent() => _cardTransform.SetParent(_transform);
        public void SetCardParent(Transform parent) => _cardTransform.SetParent(parent);
        public void SetParent(Transform parent) => _transform.SetParent(parent);
    }
}