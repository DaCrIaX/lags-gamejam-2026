namespace UnityEngine.EventSystems
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Transform _transform, _tiltTransform;

        private CanvasGroup _group;
        private CardHolder _parent;

        public CardHolder Parent => _parent;
        public Transform Transform => _transform;
        public Transform CardTransform => _tiltTransform;
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

        public void ChangeBlockRaycast(bool value) => _group.blocksRaycasts = value;
        public void RefreshParent() => _parent.RefreshCardsArray();
        public void SearchParentGroup() => Start();

        public void ResetCardParent() => _tiltTransform.SetParent(_transform);
        public void SetCardParent(Transform parent) => _tiltTransform.SetParent(parent);
        public void SetParent(Transform parent) => _transform.SetParent(parent);
    }
}