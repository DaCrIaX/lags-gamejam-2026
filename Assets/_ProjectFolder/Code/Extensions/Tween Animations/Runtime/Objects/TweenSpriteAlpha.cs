namespace UnityEngine.Animations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TweenSpriteAlpha : TweenTransparency
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

            Color color = _renderer.color;
            color.a = value;
            _renderer.color = color;
        }
    }
}