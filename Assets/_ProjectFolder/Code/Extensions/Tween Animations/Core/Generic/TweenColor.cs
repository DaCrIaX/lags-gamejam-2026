namespace UnityEngine.Animations
{
    public abstract class TweenColor : TweenCustom
    {
        [SerializeField] protected AnimationCurve _animationCurve;
        [SerializeField] protected Color _color = Color.white;

        protected Color _default = Color.white, _target = Color.white;
        protected AnimationCurve _curve;

        protected override void Awake()
        {
            base.Awake();
            _target = _color;
            _curve = _animationCurve;
        }
        protected override void OnStart()
        {
            base.OnStart();
            if (_curve.length != 0)
                OnUpdate(_current);
        }
        protected override void OnPlay(bool value)
        {
            if (_curve.length != 0)
                base.OnPlay(value);
        }

        [ContextMenu("ColorIn")] private void ColorIn() => _tweenCore.Play(true);
        [ContextMenu("ForceColorIn")] private void ColorInRepeat() => _tweenCore.ForcePlay(true);
        [ContextMenu("ColorOut")] private void ColorOut() => _tweenCore.Play(false);
        [ContextMenu("ForceColorOut")] private void ColorOutRepear() => _tweenCore.ForcePlay(false);
    }
}