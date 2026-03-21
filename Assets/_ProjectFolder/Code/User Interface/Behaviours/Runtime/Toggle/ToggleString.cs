using TMPro;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI (Canvas)/Toggle String")]
    public class ToggleString : ToggleBehaviour
    {
        [Tooltip("Require {0} to work")]
        [SerializeField] private TextMeshProUGUI _textUI;
        [SerializeField] private string _textOn, _textOff;

        private string _format;

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
                OverrideTextFormat(_textUI.text);
        }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _textUI = GetComponentInChildren<TextMeshProUGUI>();
        }
        #endif

        protected override void OnValueChanged(bool value) => _textUI?.SetText(string.Format(_format, value ? _textOn : _textOff));
        public void OverrideTextFormat(string value)
        {
            _format = value;
            OnValueChanged(isOn);
        }
    }
}