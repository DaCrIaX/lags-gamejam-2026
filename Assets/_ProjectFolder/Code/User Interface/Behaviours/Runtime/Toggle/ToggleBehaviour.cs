namespace UnityEngine.UI
{
    public abstract class ToggleBehaviour : Toggle
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
                onValueChanged.AddListener(OnValueChanged);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying)
                onValueChanged.RemoveListener(OnValueChanged);
        }
        protected abstract void OnValueChanged(bool value);

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            toggleTransition = ToggleTransition.None;
            graphic = null;
        }
        #endif
    }
}