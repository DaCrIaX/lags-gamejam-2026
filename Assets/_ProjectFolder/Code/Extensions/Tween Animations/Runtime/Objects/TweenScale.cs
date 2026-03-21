namespace UnityEngine.Animations
{
    public class TweenScale : TweenCustomVector
    {
        [SerializeField, Range(0f, 1f)] private float _normalFactor = 1f;
        [SerializeField, Range(0.5f, 1.5f)] private float _scaleFactor = 1f;

        protected override void Awake()
        {
            base.Awake();
            _from = _transform.localScale - _axis.Get() * (1f - _normalFactor);
            _to = _axis.Get() * _scaleFactor;
        }
        protected override void OnUpdate(float value)
        {
            base.OnUpdate(value);

            float time = _animationCurve.Evaluate(value);
            _transform.localScale = Vector3.LerpUnclamped(_from, _to, time);
        }

        public void ScaleIn() => _tweenCore.Play(true);
        public void ScaleOut() => _tweenCore.Play(false);
    }
}