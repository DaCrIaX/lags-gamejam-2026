using PrimeTween;

namespace UnityEngine.Animations
{
    public abstract class TweenRectPosition : TweenRectTransform
    {
        [SerializeField] protected Direction _direction;
        [SerializeField] protected Vector2 _overrideDistance;

        protected override void Awake()
        {
            base.Awake();
            var size = _rectTransform.rect.size;
            var direction = _direction.Get();

            if (_overrideDistance.x != 0) size.x = _overrideDistance.x;
            if (_overrideDistance.y != 0) size.y = _overrideDistance.y;

            _from = _to = _rectTransform.localPosition;
            _to += new Vector3(direction.x * size.x, direction.y * size.y, 0f);
        }

        protected override void OnPlay(bool value)
        {
            base.OnPlay(value);

            _tweenSettings = new(_rectTransform.localPosition, value ? _from : _to, _settings);
            _tween = Tween.LocalPosition(_rectTransform, _tweenSettings);
            _tween.OnComplete(OnComplete);
        }
    }
}