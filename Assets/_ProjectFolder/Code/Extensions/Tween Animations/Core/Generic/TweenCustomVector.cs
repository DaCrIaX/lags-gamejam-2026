namespace UnityEngine.Animations
{
    public abstract class TweenCustomVector : TweenCustom
    {
        [SerializeField] protected Axis _axis = Axis.X | Axis.Y | Axis.Z;
        [SerializeField] protected AnimationCurve _animationCurve;

        protected Transform _transform;
        protected Vector3 _from, _to;

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
        }
        protected override void OnStart()
        {
            base.OnStart();
            if (_animationCurve.length != 0)
                OnUpdate(_current);
        }
        protected override void OnPlay(bool value)
        {
            if (_animationCurve.length != 0)
                base.OnPlay(value);
        }
    }
}