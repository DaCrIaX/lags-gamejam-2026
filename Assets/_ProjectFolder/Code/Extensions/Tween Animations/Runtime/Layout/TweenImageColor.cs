namespace UnityEngine.Animations
{
    using UI;

    [RequireComponent(typeof(Image))]
    public class TweenImageColor : TweenColor
    {
        private Image _image;

        protected override void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            _default = _image.color;
        }
        protected override void OnUpdate(float value)
        {
            base.OnUpdate(value);
            float time = _curve.Evaluate(value);
            _image.color = Color.Lerp(_default, _target, time);
        }
    }
}