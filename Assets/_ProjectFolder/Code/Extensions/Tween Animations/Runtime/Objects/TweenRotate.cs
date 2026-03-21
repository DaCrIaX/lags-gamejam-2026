namespace UnityEngine.Animations
{
    public class TweenRotate : TweenCustomVector
    {
        [SerializeField] protected float _angle;

        protected override void Awake()
        {
            base.Awake();
            _from = _to = _transform.localEulerAngles;
            _to += _axis.Get() * -_angle;
        }
        protected override void OnUpdate(float value)
        {
            base.OnUpdate(value);

            float time = _animationCurve.Evaluate(value);
            _transform.localEulerAngles = Vector3.Lerp(_from, _to, time);
        }

        [ContextMenu("FlipIn")]
        public void RotateIn() => _tweenCore?.Play(true);

        [ContextMenu("FlipOut")]
        public void RotateOut() => _tweenCore?.Play(false);
    }
}