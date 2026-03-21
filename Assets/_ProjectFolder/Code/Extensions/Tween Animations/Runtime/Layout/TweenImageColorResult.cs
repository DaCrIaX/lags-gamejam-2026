namespace UnityEngine.Animations
{
    public class TweenImageColorResult : TweenImageColor
    {
        [Header("Alternative Color")]
        [SerializeField] private AnimationCurve _altAnimationCurve;
        [SerializeField] private Color _altColor = Color.red;

        protected override void OnPlay(bool value)
        {
            _current = 0;
            _target = value ? _color : _altColor;
            _curve = value ? _animationCurve : _altAnimationCurve;

            base.OnPlay(true);
        }
    }
}