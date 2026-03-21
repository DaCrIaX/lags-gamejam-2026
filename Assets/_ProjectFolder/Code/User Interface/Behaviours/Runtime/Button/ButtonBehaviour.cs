namespace UnityEngine.UI
{
    public abstract class ButtonBehaviour : Button
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
                onClick.AddListener(OnClick);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying)
                onClick.RemoveListener(OnClick);
        }

        protected abstract void OnClick();
    }
}