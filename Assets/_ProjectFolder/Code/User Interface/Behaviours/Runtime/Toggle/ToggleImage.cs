namespace UnityEngine.UI
{
    [AddComponentMenu("UI (Canvas)/Toggle Image")]
    public class ToggleImage : ToggleBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _onSprite, _offSprite;

        protected override void OnValueChanged(bool isOn)
        {
            if (_icon)
                _icon.sprite = isOn ? _onSprite : _offSprite;
        }
    }
}