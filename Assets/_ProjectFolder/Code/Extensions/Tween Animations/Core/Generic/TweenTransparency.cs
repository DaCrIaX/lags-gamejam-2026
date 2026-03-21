namespace UnityEngine.Animations
{
    [DefaultExecutionOrder(-25)]
    public abstract class TweenTransparency : TweenCustom
    {
        [SerializeField] private bool _disableOnHidden;

        protected override void OnStart()
        {
            base.OnStart();
            OnUpdate(_current);
        }
        protected override void OnComplete()
        {
            base.OnComplete();

            if (_current == 0)
                PerformVisibility(false);
        }
        protected override void OnPlay(bool value)
        {
            PerformVisibility(true);
            base.OnPlay(value);
        }

        private void PerformVisibility(bool value)
        {
            if (_disableOnHidden)
                gameObject.SetActive(value);
        }

        [ContextMenu("FadeIn")]
        public void FadeIn()
        {
            PerformVisibility(true);
            _tweenCore?.Play(true);
        }

        [ContextMenu("FadeOut")]
        public void FadeOut()
        {
            _tweenCore?.Play(false);
        }
    }
}