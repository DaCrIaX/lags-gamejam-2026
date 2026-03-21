using TMPro;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI (Canvas)/Button String")]
    public class ButtonString : ButtonBehaviour
    {
        [Tooltip("Require {0} to work")]
        [SerializeField] private TextMeshProUGUI _textUI;
        [SerializeField] private string _text;

        private string _format;

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
                _textUI?.SetText(string.Format(_format, _text));
        }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _textUI = GetComponentInChildren<TextMeshProUGUI>();
        }
        #endif

        protected override void OnClick() { }
        public void OverrideTextFormat(string value)
        {
            _format = value;
            _textUI?.SetText(string.Format(_format, _text));
        }
    }
}