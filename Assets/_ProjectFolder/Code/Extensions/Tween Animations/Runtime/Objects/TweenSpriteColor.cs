namespace UnityEngine.Animations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TweenSpriteColor : TweenColor
    {
        private SpriteRenderer _renderer;

        protected override void Awake()
        {
            base.Awake();
            _renderer = GetComponent<SpriteRenderer>();
        }
        protected override void OnUpdate(float value)
        {
            base.OnUpdate(value);
            float time = _curve.Evaluate(value);
            _renderer.color = Color.Lerp(_default, _target, time);
        }
    }
}