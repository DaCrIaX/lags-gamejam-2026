namespace UnityEngine.Animations
{
    public class TweenScale : TweenCustomVector
    {
        [SerializeField, Range(0f, 1f)] private float _normalFactor = 1f;

        [Header("Modify")]
        [SerializeField] private Axis _axisFactor = Axis.X | Axis.Y | Axis.Z;
        [SerializeField, Range(0.5f, 1.5f)] private float _scaleFactor = 1f;

        protected override void Awake()
        {
            base.Awake();
            _from = _transform.localScale - _axis.Get() * (1f - _normalFactor);
            _to = _axisFactor.Get() * _scaleFactor;
        }
        protected override void OnUpdate(float value)
        {
            base.OnUpdate(value);

            float time = _animationCurve.Evaluate(value);
            _transform.localScale = Vector3.LerpUnclamped(_from, _to, time);
        }

        [ContextMenu("Scale In")]
        public void ScaleIn() => _tweenCore.Play(true);

        [ContextMenu("Scale Out")]
        public void ScaleOut() => _tweenCore.Play(false);
    }
}